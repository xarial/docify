//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

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
