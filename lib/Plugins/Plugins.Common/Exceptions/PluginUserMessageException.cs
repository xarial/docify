using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Lib.Plugins.Common.Exceptions
{
    public class PluginUserMessageException : Exception, IUserMessageException
    {
        public PluginUserMessageException(string message) : base(message)
        {
        }

        public PluginUserMessageException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
