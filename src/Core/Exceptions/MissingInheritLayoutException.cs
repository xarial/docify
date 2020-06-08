using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class MissingInheritLayoutException : UserMessageException
    {
        public MissingInheritLayoutException(string path, string layoutName) 
            : base($"Inherited layout is not available for {layoutName} layout in '{path}'. Make sure that at least one parent page has an assigned layout so it can be inherited") 
        {
        }
    }
}
