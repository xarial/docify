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
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ObservableCodeBlockExtension : IMarkdownExtension
    {
        private readonly IEnumerable<IRenderCodeBlockPlugin> m_Plugins;

        public ObservableCodeBlockExtension(IEnumerable<IRenderCodeBlockPlugin> plugins)
        {
            m_Plugins = plugins;
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
                    new ObservableCodeBlockRenderer(m_Plugins));
            }
            else 
            {
                throw new NotSupportedException("Renderer is not upported");
            }
        }
    }
}
