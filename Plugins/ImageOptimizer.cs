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

namespace Xarial.Docify.Lib.Plugins
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
    public class ImageOptimizer : IPreCompilePlugin, IPrePublishFilePlugin, IPlugin<ImageOptimizerSettings>
    {
        private const string IMAGE_TAG_NAME = "image";
        private const string REPLACE_IMAGE_TAG_NAME = "image-png";
        private const string SVG_EXT = ".svg";
        private const string PNG_EXT = ".png";

        private ImageOptimizerSettings m_Settings;

        public void Init(ImageOptimizerSettings setts)
        {
            m_Settings = setts;
        }

        public void PreCompile(ISite site)
        {
            System.Diagnostics.Debugger.Launch();

            foreach (var page in site.GetAllPages()) 
            {
                string image;

                if (MetadataExtension.TryGetParameter<string>(page.Data, IMAGE_TAG_NAME, out image))
                {
                    if (!string.IsNullOrEmpty(image))
                    {
                        if (string.Equals(Path.GetExtension(image), 
                            SVG_EXT, StringComparison.CurrentCultureIgnoreCase)) 
                        {
                            var imgAsset = TryFindImageAsset(site, page, image);

                            if (imgAsset == null)
                            {
                                throw new NullReferenceException($"Failed to find image asset: {image}");
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
                            var imgPngAsset = new PluginDataFile(pngBuffer, new PluginDataFileLocation(imgName, page.Location.Path.ToArray()));
                            page.Assets.Add(imgPngAsset);
                        }
                    }
                }
            }

            if (m_Settings.GenerateFavIcon)
            {
                GenerateFavIcon(site);
            }
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

        private IFile TryFindImageAsset(ISite site, IPage page, string path)
        {
            if (!path.StartsWith('/')) 
            {
                path = page.Location.ToUrl().TrimEnd('/') + "/" + path;
            }

            return site.MainPage.Assets.FirstOrDefault(
                    p => string.Equals(p.Location.ToUrl(), path)
                    || string.Equals(p.Location.ToUrl(site.BaseUrl), path));
        }
        
        public void PrePublishFile(ILocation outLoc, ref IFile file, out bool skip)
        {
            skip = false;

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
                                    file = new PluginDataFile(outStr.GetBuffer(), file.Location);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
