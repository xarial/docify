using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins
{
    public class File : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }

        public File(string content, ILocation loc) 
            : this(FileExtension.ToByteArray(content), loc)
        {
        }

        public File(byte[] content, ILocation loc)
        {
            Content = content;
            Location = loc;
        }
    }
}
