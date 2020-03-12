using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Lib.Plugins
{
    [Plugin("image-aligner")]
    public class ImageAlignerPlugin : IRenderImagePlugin
    {
        public void RenderImage(string content, ref string result)
        {
            Debug.Assert(false);
        }
    }
}
