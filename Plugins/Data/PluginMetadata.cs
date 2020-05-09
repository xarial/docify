using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Data
{
    public class PluginMetadata : Dictionary<string, object>, IMetadata
    {
        public PluginMetadata() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public PluginMetadata(IDictionary<string, object> parameters)
            : base(parameters, StringComparer.CurrentCultureIgnoreCase)
        {
        }

        public virtual IMetadata Copy(IDictionary<string, object> data)
        {
            return new PluginMetadata(data);
        }
    }
}
