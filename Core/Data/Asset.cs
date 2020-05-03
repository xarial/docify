using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public class Asset : IFile
    {
        public static Asset FromTextContent(string content, Location loc)
        {
            var buffer = FileExtension.ToByteArray(content);

            return new Asset(loc, buffer);
        }

        public byte[] Content { get; }
        public ILocation Location { get; }

        public Asset(ILocation loc, byte[] content)
        {
            Location = loc;
            Content = content;
        }
    }
}
