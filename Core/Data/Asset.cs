using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;

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
        public Location Location { get; }

        public Asset(Location loc, byte[] content)
        {
            Location = loc;
            Content = content;
        }
    }
}
