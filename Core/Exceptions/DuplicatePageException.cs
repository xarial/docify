//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicatePageException : Exception
    {
        public DuplicatePageException(Location loc) 
            : base($"Specified page already exist '{loc.ToId()}'")
        {
        }
    }
}
