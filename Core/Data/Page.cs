﻿using System;
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
        public List<IFile> Assets { get; }
        public ILocation Location { get; }

        public override string Key => Location.ToId();

        public Page(ILocation url, string rawContent, Template layout = null)
            : this(url, rawContent, new Metadata(), layout)
        {

        }

        public Page(ILocation url, string rawContent, IMetadata data, Template layout = null)
            : base(rawContent, data, layout)
        {
            Location = url;
            SubPages = new List<IPage>();
            Assets = new List<IFile>();
        }
    }
}