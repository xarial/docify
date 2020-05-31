//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Markdig;
using Markdig.Renderers;
using System;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableLinkExtension : IMarkdownExtension
    {
        private readonly ICompilerExtension m_Ext;

        public ObservableLinkExtension(ICompilerExtension ext)
        {
            m_Ext = ext;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.TryRemove<Markdig.Parsers.IndentedCodeBlockParser>();
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;

            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.ReplaceOrAdd<Markdig.Renderers.Html.Inlines.LinkInlineRenderer>(
                    new ObservableLinkInlineRenderer(m_Ext));
            }
            else
            {
                throw new NotSupportedException("Renderer is not upported");
            }
        }
    }
}
