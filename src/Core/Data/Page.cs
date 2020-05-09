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
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Page : Sheet, IPage
    {
        public List<IPage> SubPages { get; }
        public List<IAsset> Assets { get; }
        public List<IAssetsFolder> Folders { get; }

        private static IMetadata ComposePageMetadata(IMetadata data, ITemplate layout)
        {
            if (layout != null)
            {
                return data.Merge(layout.Data);
            }
            else
            {
                return data;
            }
        }

        public Page(string name, string rawContent, IMetadata data, string id, ITemplate layout = null)
            : base(rawContent, name, ComposePageMetadata(data, layout), layout, id)
        {
            SubPages = new List<IPage>();
            Assets = new List<IAsset>();
            Folders = new List<IAssetsFolder>();
        }
    }
}
