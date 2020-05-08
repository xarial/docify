using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Data
{
    public class PluginDataFile : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }

        public PluginDataFile(string content, ILocation loc) 
            : this(ContentExtension.ToByteArray(content), loc)
        {
        }

        public PluginDataFile(byte[] content, ILocation loc)
        {
            Content = content;
            Location = loc;
        }
    }
}
