//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class IncludeResolveException : Exception
    {
        public IncludeResolveException(string name, string url, Exception inner)
            : base($"Failed to resolve include '{name}' in '{url}'", inner)
        {
        }
    }
}
