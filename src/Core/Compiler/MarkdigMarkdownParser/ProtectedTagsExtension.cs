//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using Markdig.Extensions.GenericAttributes;
using Markdig.Renderers;
using System;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ProtectedTagsExtension : IMarkdownExtension
    {
        private readonly string m_StartTag;
        private readonly string m_EndTag;

        public ProtectedTagsExtension(string startTag, string endTag)
        {
            m_StartTag = startTag;
            m_EndTag = endTag;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<ProtectedTagsInlineParser>())
            {
                pipeline.InlineParsers.InsertBefore<GenericAttributesParser>(
                    new ProtectedTagsInlineParser(m_StartTag, m_EndTag));
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;

            if (htmlRenderer != null)
            {
                htmlRenderer.ObjectRenderers.AddIfNotAlready<ProtectedTagsRenderer>();
            }
            else
            {
                throw new NotSupportedException("Renderer is not upported");
            }
        }
    }
}
