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
