//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableLinkInlineRenderer : Markdig.Renderers.Html.Inlines.LinkInlineRenderer
    {
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            base.Write(renderer, link);
        }
    }
}
