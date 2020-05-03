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
    public class ImageOptimizer : IPreCompilePlugin, IPlugin<ImageOptimizerSettings>
    {
        private const string IMAGE_TAG_NAME = "image";
        private const string REPLACE_IMAGE_TAG_NAME = "image-png";
        private const string SVG_EXT = ".svg";
        private const string PNG_EXT = ".png";

        public ImageOptimizerSettings Settings { get; set; }

        public void PreCompile(ISite site)
        {
            foreach (var page in site.GetAllPages()) 
            {
                dynamic imageVal;
                if (page.Data.TryGetValue(IMAGE_TAG_NAME, out imageVal)) 
                {
                    var image = imageVal as string;

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
                                var bitmap = svgDocument.Draw(Settings.SvgPngWidth, Settings.SvgPngHeight);

                                using (var pngStream = new MemoryStream()) 
                                {
                                    bitmap.Save(pngStream, ImageFormat.Png);
                                    pngBuffer = pngStream.ToArray();
                                }
                            }

                            page.Data.Add(REPLACE_IMAGE_TAG_NAME, imgName);
                            var imgPngAsset = new File(pngBuffer, new Location(imgName, page.Location.Path.ToArray()));
                            page.Assets.Add(imgPngAsset);
                            site.MainPage.Assets.Add(imgPngAsset);
                        }
                    }
                }
            }

            if (Settings.GenerateFavIcon)
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

        public void PrePublishAsset(ref Location loc, ref byte[] content, out bool cancel)
        {
            cancel = false;

            var path = loc.ToPath();

            var opts = Settings.IgnoreMatchCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            if (Settings.MatchPattern.Any(p => Regex.IsMatch(path, p, opts)))
            {
                var quantizer = new WuQuantizer();

                using (var inStr = new MemoryStream(content))
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
                                    content = outStr.GetBuffer();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
