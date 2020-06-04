//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class LibraryItemLoadException : UserMessageException
    {
        public LibraryItemLoadException(string itemName, string loc, Exception inner)
            : base($"Failed to load library item: '{itemName}' from '{loc}'", inner)
        {
        }
    }
}
