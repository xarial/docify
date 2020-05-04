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
        public byte[] Content { get; }
        public ILocation Location { get; }

        public Asset(ILocation loc, byte[] content)
        {
            Location = loc;
            Content = content;
        }

        public Asset(ILocation loc, string content) : this(loc, FileExtension.ToByteArray(content))
        {
        }
    }
}
