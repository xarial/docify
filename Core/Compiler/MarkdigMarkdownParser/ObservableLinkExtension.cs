//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using Markdig.Renderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableLinkExtension : IMarkdownExtension
    {
        public ObservableLinkExtension()
        {
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;

            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.ReplaceOrAdd<Markdig.Renderers.Html.Inlines.LinkInlineRenderer>(
                    new ObservableLinkInlineRenderer());
            }
        }
    }
}
