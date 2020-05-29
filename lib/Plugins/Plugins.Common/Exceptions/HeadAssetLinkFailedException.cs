using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Lib.Plugins.Exceptions
{
    public class HeadAssetLinkFailedException : Exception
    {
        public HeadAssetLinkFailedException(string asset, string url, Exception inner) 
            : base($"Failed to insert '{asset}' link to '{url}'", inner) 
        {
        }
    }
}
