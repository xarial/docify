using System;
using System.Collections.Generic;
using System.Text;

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
