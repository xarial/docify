//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class IncludeData : LeafInline
    {
        public string Name { get; }
        public Dictionary<string, dynamic> Parameters { get; }
        public Site Site { get; }
        public Page Page { get; }

        public IncludeData(string name, Dictionary<string, dynamic> parameters, Site site, Page page)
        {
            Name = name;
            Parameters = parameters;
            Site = site;
            Page = page;
        }
    }
}
