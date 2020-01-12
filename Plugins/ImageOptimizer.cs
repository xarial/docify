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

namespace Plugins
{
    [Plugin("image-optimizer")]
    public class ImageOptimizer : IPrePublishBinaryAssetPlugin
    {
        private readonly string[] m_ImageExtensions = new string[]
        {
            ".png", ".jpg", ".jpeg", ".bmp"
        };

        public void PrePublishBinaryAsset(ref Location loc, ref byte[] content, out bool cancel)
        {
            cancel = false;

            if (m_ImageExtensions.Contains(Path.GetExtension(loc.FileName),
                StringComparer.CurrentCultureIgnoreCase)) 
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
