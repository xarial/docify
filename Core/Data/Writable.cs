using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;

namespace Xarial.Docify.Core.Data
{
    public class Writable : IFile
    {
        public byte[] Content { get; }

        public Location Location { get; }

        public Writable(byte[] content, Location loc)
        {
            Content = content;
            Location = loc;
        }

        public Writable(string content, Location loc)
            : this(FileExtension.ToByteArray(content), loc)
        {
        }
    }
}
