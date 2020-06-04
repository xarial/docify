using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Exceptions
{
    public class DuplicatePluginException : UserMessageException
    {
        public DuplicatePluginException(string pluginId) 
            : base($"Plugin '{pluginId}' contains more than one plugin") 
        {
        }
    }
}
