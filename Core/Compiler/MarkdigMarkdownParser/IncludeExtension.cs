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
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class IncludeExtension : IMarkdownExtension
    {
        private readonly IIncludesHandler m_ParamsParser;

        public IncludeExtension(IIncludesHandler paramsParser)
        {
            m_ParamsParser = paramsParser;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.AddIfNotAlready(new IncludeInlineParser(m_ParamsParser));
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready(new IncludeRenderer(m_ParamsParser));
            }
        }
    }
}
