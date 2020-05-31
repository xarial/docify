//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicateFileException : Exception
    {
        public DuplicateFileException(string loc, string relPath)
            : base($"Failed to load '{relPath}' from '{loc}' as this reletive path was already loaded from different location")
        {
        }
    }
}
