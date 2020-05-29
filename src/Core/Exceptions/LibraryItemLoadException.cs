//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class LibraryItemLoadException : Exception
    {
        public LibraryItemLoadException(string itemName, string loc, Exception inner) 
            : base($"Failed to load library item: '{itemName}' from '{loc}'", inner) 
        {
        }
    }
}
