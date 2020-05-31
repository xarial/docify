//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;

namespace Xarial.Docify.Lib.Plugins.Common.Exceptions
{
    public class HeadAssetLinkFailedException : Exception
    {
        public HeadAssetLinkFailedException(string asset, string url, Exception inner)
            : base($"Failed to insert '{asset}' link to '{url}'", inner)
        {
        }
    }
}
