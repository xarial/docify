//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Data
{
    public class PluginAsset : IAsset
    {
        public byte[] Content { get; }
        public string FileName { get; }

        public string Id { get; }

        public PluginAsset(string content, string name) 
            : this(ContentExtension.ToByteArray(content), name)
        {
        }

        public PluginAsset(byte[] content, string name)
            : this(content, name, Guid.NewGuid().ToString())
        {
        }

        public PluginAsset(byte[] content, string name, string id)
        {
            Content = content;
            FileName = name;
            Id = id;
        }
    }
}
