//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Markdig.Renderers;
using Markdig.Renderers.Html;

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
