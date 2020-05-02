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
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public class Page : Frame
    {
        public List<Page> SubPages { get; }
        public List<Asset> Assets { get; }
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
            SubPages = new List<Page>();
            Assets = new List<Asset>();
        }
    }

    public static class PageExtension
    {
        public static IEnumerable<Page> GetAllSubPages(this Page page)
        {
            if (page.SubPages != null)
            {
                foreach (var childPage in page.SubPages)
                {
                    yield return childPage;

                    foreach (var subChildPage in GetAllSubPages(childPage))
                    {
                        yield return subChildPage;
                    }
                }
            }
        }
    }
}
