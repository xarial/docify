using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class DigitalSignatureMismatchException : Exception
    {
        public DigitalSignatureMismatchException(ILocation loc) 
            : base($"Digital signature of the '{loc.ToId()}' doesn't match the file manifest")
        {
        }
    }
}
