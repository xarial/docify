using System;
using System.Collections.Generic;
using System.Text;

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
