//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
