using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Data
{
    public class PluginAsset : IAsset
    {
        public byte[] Content { get; }
        public string FileName { get; }

        public PluginAsset(string content, string name) 
            : this(ContentExtension.ToByteArray(content), name)
        {
        }

        public PluginAsset(byte[] content, string name)
        {
            Content = content;
            FileName = name;
        }
    }
}
