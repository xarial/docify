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

namespace Xarial.Docify.Lib.Plugins
{
    public class ImageOptimizerSettings 
    {
        [DataMember(Name = "match-pattern")]
        public string[] MatchPatterns { get; set; } = new string[]
        {
            "\\.png$", "\\.jpg$", "\\.jpeg$", "\\.bmp$", "\\.tif$", "\\.tiff$"
        };

        [DataMember(Name = "ignore-match-case")]
        public bool IgnoreMatchCase { get; set; } = true;
    }

    [Plugin("image-optimizer")]
    public class ImageOptimizer : IPrePublishBinaryAssetPlugin, IPlugin<ImageOptimizerSettings>
    {        
        public ImageOptimizerSettings Settings { get; set; }

        public void PrePublishBinaryAsset(ref Location loc, ref byte[] content, out bool cancel)
        {
            cancel = false;

            var path = loc.ToPath();

            var opts = Settings.IgnoreMatchCase ? RegexOptions.IgnoreCase : RegexOptions.None;

            if (Settings.MatchPatterns.Any(p => Regex.IsMatch(path, p, opts)))
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
