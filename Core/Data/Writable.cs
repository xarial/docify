using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Writable : IFile
    {
        public byte[] Content { get; }

        public ILocation Location { get; }

        public Writable(byte[] content, ILocation loc)
        {
            Content = content;
            Location = loc;
        }

        public Writable(string content, ILocation loc)
            : this(FileExtension.ToByteArray(content), loc)
        {
        }
    }
}
