//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

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
