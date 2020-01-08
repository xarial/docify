//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class SiteSource : ISiteSource
    {
        public IEnumerable<IPageSource> Pages { get; }
        public IEnumerable<IInclude> Includes { get; }
        public IEnumerable<ILayout> Layouts { get; }
        public IEnumerable<IAssetSource> Assets { get; }
        public string Path { get; }

        public SiteSource(string path, IEnumerable<IPageSource> pages, IEnumerable<IInclude> includes, IEnumerable<ILayout> layouts, IEnumerable<IAssetSource> assets) 
        {
            Path = path;
            Pages = pages ?? Enumerable.Empty<IPageSource>();
            Includes = includes ?? Enumerable.Empty<IInclude>();
            Layouts = layouts ?? Enumerable.Empty<ILayout>();
            Assets = assets ?? Enumerable.Empty<IAssetSource>();
        }
    }
}
