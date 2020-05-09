using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Data
{
    public class PluginFile : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }

        public PluginFile(string content, ILocation loc) 
            : this(ContentExtension.ToByteArray(content), loc)
        {
        }

        public PluginFile(byte[] content, ILocation loc)
        {
            Content = content;
            Location = loc;
        }
    }
}
