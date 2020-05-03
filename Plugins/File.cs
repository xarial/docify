using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;

namespace Xarial.Docify.Lib.Plugins
{
    public class File : IFile
    {
        public byte[] Content { get; }
        public Location Location { get; }

        public File(string content, Location loc) 
            : this(FileExtension.ToByteArray(content), loc)
        {
        }

        public File(byte[] content, Location loc)
        {
            Content = content;
            Location = loc;
        }
    }
}
