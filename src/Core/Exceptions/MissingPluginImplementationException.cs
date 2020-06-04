using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Exceptions
{
    public class MissingPluginImplementationException : UserMessageException
    {
        public MissingPluginImplementationException(IEnumerable<string> pluginNames) 
            : base($"{string.Join(", ", pluginNames)} plugins were not loaded. Make sure that there is public class which implements {typeof(IPlugin).FullName} or {typeof(IPlugin<>).FullName} interface")
        {
        }
    }
}
