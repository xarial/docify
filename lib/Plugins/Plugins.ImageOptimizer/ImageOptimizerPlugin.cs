﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Plugins;
using System.Linq;
using System.IO;
using nQuant;
using System.Drawing;
using System.Text.RegularExpressions;
using Svg;
using System.Drawing.Imaging;
using Xarial.Docify.Base.Data;
using System.Threading.Tasks;
using Xarial.Docify.Lib.Plugins.Common.Helpers;
using Xarial.Docify.Lib.Plugins.Common.Data;

namespace Xarial.Docify.Lib.Plugins.ImageOptimizer
{
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
            foreach (var page in site.MainPage.GetAllPages())
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

        private Task OnPrePublishFile(ILocation outLoc, PrePublishFileArgs args)
        {
            var path = args.File.Location.ToPath();

            var opts = m_Settings.IgnoreMatchCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            if (m_Settings.MatchPattern.Any(p => Regex.IsMatch(path, p, opts)))
            {
                var quantizer = new WuQuantizer();

                using (var inStr = new MemoryStream(args.File.Content))
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
                                    args.File = new PluginFile(outStr.GetBuffer(), args.File.Location);
                                }
                            }
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
