//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using Markdig.Renderers;
using System;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableCodeBlockExtension : IMarkdownExtension
    {
        private readonly ICompilerExtension m_Ext;

        public ObservableCodeBlockExtension(ICompilerExtension ext)
        {
            m_Ext = ext;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;

            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.ReplaceOrAdd<Markdig.Renderers.Html.CodeBlockRenderer>(
                    new ObservableCodeBlockRenderer(m_Ext));
            }
            else
            {
                throw new NotSupportedException("Renderer is not upported");
            }
        }
    }
}
