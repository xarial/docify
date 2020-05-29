//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Plugins;
using System.Linq;
using System.IO;
using nQuant;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using Svg;
using System.Drawing.Imaging;
using System.Collections;
using System.Collections.Generic;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Lib.Plugins.Data;
using Xarial.Docify.Lib.Plugins.Helpers;
using System.Threading.Tasks;

namespace Xarial.Docify.Lib.Plugins.ImageOptimizer
{
    public class ImageOptimizerSettings 
    {
        public string[] MatchPattern { get; set; } = new string[]
        {
            "\\.png$", "\\.jpg$", "\\.jpeg$", "\\.bmp$", "\\.tif$", "\\.tiff$"
        };

        public bool IgnoreMatchCase { get; set; } = true;

        public bool ImageTagConvertSvgToPng { get; set; } = true;

        /// <remarks>
        /// 0 to maintain aspect ration
        /// </remarks>
        public int SvgPngWidth { get; set; } = 1200;

        /// <remarks>
        /// 0 to maintain aspect ration
        /// </remarks>
        public int SvgPngHeight { get; set; } = 0;

        public bool GenerateFavIcon { get; set; } = false;
    }

    [Plugin("image-optimizer")]
    public class ImageOptimizerPlugin : IPlugin<ImageOptimizerSettings>
    {
        private const string IMAGE_TAG_NAME = "image";
        private const string REPLACE_IMAGE_TAG_NAME = "image-png";
        private const string SVG_EXT = ".svg";
        private const string PNG_EXT = ".png";

        private ImageOptimizerSettings m_Settings;

        private IDocifyApplication m_App;

        public void Init(IDocifyApplication app, ImageOptimizerSettings setts)
        {
            m_App = app;
            m_Settings = setts;

            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Publisher.PrePublishFile += OnPrePublishFile;
        }

        private Task OnPreCompile(ISite site)
        {
            foreach (var page in AssetsHelper.GetAllPages(site.MainPage)) 
            {
                string image;

                if (MetadataExtension.TryGetParameter<string>(page.Data, IMAGE_TAG_NAME, out image))
                {
                    if (!string.IsNullOrEmpty(image))
                    {
                        if (string.Equals(Path.GetExtension(image), 
                            SVG_EXT, StringComparison.CurrentCultureIgnoreCase)) 
                        {
                            IAsset imgAsset;

                            try
                            {
                                imgAsset = AssetsHelper.FindAsset(site, page, image);
                            }
                            catch (Exception ex)
                            {
                                throw new NullReferenceException($"Failed to find image asset: '{image}'", ex);
                            }
                            
                            var imgName = Path.GetFileNameWithoutExtension(image) + PNG_EXT;

                            byte[] pngBuffer = null;
                            
                            using (var svgStream = new MemoryStream(imgAsset.Content))
                            {
                                var svgDocument = SvgDocument.Open<SvgDocument>(svgStream);
                                var bitmap = svgDocument.Draw(m_Settings.SvgPngWidth, m_Settings.SvgPngHeight);

                                using (var pngStream = new MemoryStream()) 
                                {
                                    bitmap.Save(pngStream, ImageFormat.Png);
                                    pngBuffer = pngStream.ToArray();
                                }
                            }

                            page.Data.Add(REPLACE_IMAGE_TAG_NAME, imgName);
                            var imgPngAsset = new PluginAsset(pngBuffer, imgName);
                            page.Assets.Add(imgPngAsset);
                        }
                    }
                }
            }

            if (m_Settings.GenerateFavIcon)
            {
                GenerateFavIcon(site);
            }

            return Task.CompletedTask;
        }

        private void GenerateFavIcon(ISite site) 
        {
            //TODO: implement
            throw new NotImplementedException();

            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = null;
                Icon.FromHandle(bitmap.GetHicon()).Save(stream);
            }
        }

        private Task<PrePublishResult> OnPrePublishFile(ILocation outLoc, IFile file)
        {
            var res = new PrePublishResult()
            {
                File = file,
                SkipFile = false
            };

            var path = file.Location.ToPath();

            var opts = m_Settings.IgnoreMatchCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            if (m_Settings.MatchPattern.Any(p => Regex.IsMatch(path, p, opts)))
            {
                var quantizer = new WuQuantizer();

                using (var inStr = new MemoryStream(file.Content))
                {
                    inStr.Seek(0, SeekOrigin.Begin);

                    using (var img = Image.FromStream(inStr))
                    {
                        using (var bmp = new Bitmap(img))
                        {
                            using (var outStr = new MemoryStream())
                            {
                                using (var quantized = quantizer.QuantizeImage(bmp))
                                {
                                    quantized.Save(outStr, img.RawFormat);
                                    outStr.Seek(0, SeekOrigin.Begin);
                                    res.File = new PluginFile(outStr.GetBuffer(), file.Location);
                                }
                            }
                        }
                    }
                }
            }

            return Task.FromResult(res);
        }
    }
}
