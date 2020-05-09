using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Page : Sheet, IPage
    {
        public List<IPage> SubPages { get; }
        public List<IAsset> Assets { get; }
        public List<IAssetsFolder> Folders { get; }
        
        public Page(string name, string rawContent, IMetadata data, string id, Template layout = null)
            : base(rawContent, name, data, layout, id)
        {
            SubPages = new List<IPage>();
            Assets = new List<IAsset>();
            Folders = new List<IAssetsFolder>();
        }
    }
}
