using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public class Page : Frame, IPage
    {
        public List<IPage> SubPages { get; }
        public List<IFile> Assets { get; }
        public Location Location { get; }

        public override string Key => Location.ToId();

        public Page(Location url, string rawContent, Template layout = null)
            : this(url, rawContent, new Metadata(), layout)
        {

        }

        public Page(Location url, string rawContent, Metadata data, Template layout = null)
            : base(rawContent, data, layout)
        {
            Location = url;
            SubPages = new List<IPage>();
            Assets = new List<IFile>();
        }
    }
}
