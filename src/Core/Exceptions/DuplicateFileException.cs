//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicateFileException : UserMessageException
    {
        public DuplicateFileException(string loc, string relPath)
            : base($"Failed to load '{relPath}' from '{loc}' as this reletive path was already loaded from different location")
        {
        }
    }
}
