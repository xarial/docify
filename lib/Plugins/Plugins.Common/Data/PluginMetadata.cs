//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Data
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
