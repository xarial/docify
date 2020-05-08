using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Page : Frame, IPage
    {
        public List<IPage> SubPages { get; }
        public List<IAsset> Assets { get; }
        public List<IAssetsFolder> Folders { get; }

        public Page(string rawContent, string name, Template layout = null)
            : this(rawContent, "", new Metadata(), layout)
        {

        }

        public Page(string rawContent, string name, IMetadata data, Template layout = null)
            : base(rawContent, name, data, layout)
        {
            SubPages = new List<IPage>();
            Assets = new List<IAsset>();
            Folders = new List<IAssetsFolder>();
        }
    }
}
