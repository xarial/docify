using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class MetadataDeserializationException : UserMessageException
    {
        public MetadataDeserializationException(string msg, Exception inner) : base(msg, inner) 
        {
        }
    }
}
