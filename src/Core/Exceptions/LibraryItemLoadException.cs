//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class LibraryItemLoadException : UserMessageException
    {
        public LibraryItemLoadException(ILocation loc)
            : base($"Failed to load library item: '{loc.ToId()}'")
        {
        }
    }
}
