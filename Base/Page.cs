//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Content;

namespace Xarial.Docify.Base
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public class Page : Frame
    {
        public List<Page> Children { get; }
        public List<Asset> Assets { get; }
        public Location Location { get; }
        public string Content { get; set; }

        public override string Key => Location.ToId();

        public Page(Location url, string rawContent, Template layout = null)
            : this(url, rawContent, new Dictionary<string, dynamic>(), layout)
        {

        }

        public Page(Location url, string rawContent, Dictionary<string, dynamic> data, Template layout = null)
            : base(rawContent, data, layout)
        {
            Location = url;
            Children = new List<Page>();
            Assets = new List<Asset>();
        }
    }
}
