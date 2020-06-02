using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Exceptions
{
    public class UserMessageException : Exception, IUserMessageException
    {
        public UserMessageException(string message) : base(message)
        {
        }

        public UserMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
