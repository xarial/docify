//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ProtectedTagsRenderer : HtmlObjectRenderer<ProtectedTagsData>
    {
        public ProtectedTagsRenderer()
        {
        }

        protected override void Write(HtmlRenderer renderer, ProtectedTagsData data)
        {
            renderer.Write(data.Content);
        }
    }
}
