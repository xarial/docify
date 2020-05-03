using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;

namespace Xarial.Docify.Core.Data
{
    public class Writable : IFile
    {
        public static Writable FromTextContent(string content, Location loc)
        {
            var buffer = FileExtension.ToByteArray(content);

            return new Writable(buffer, loc);
        }

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
