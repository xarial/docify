using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins
{
    public class PluginReplacedFile : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }

        public PluginReplacedFile(string content, ILocation loc) 
            : this(FileExtension.ToByteArray(content), loc)
        {
        }

        public PluginReplacedFile(byte[] content, ILocation loc)
        {
            Content = content;
            Location = loc;
        }
    }
}
