//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Lib.Tools.Exceptions
{
    public class RootPageNotFoundException : Exception
    {
        public RootPageNotFoundException(string url)
            : base($"Specified root page '{url}' is not found")
        {
        }
    }
}
