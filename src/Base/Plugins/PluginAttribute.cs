//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Plugins
{
    public class PluginAttribute : Attribute
    {
        public string Id { get; private set; }

        public PluginAttribute(string id)
        {
            Id = id;
        }
    }
}
