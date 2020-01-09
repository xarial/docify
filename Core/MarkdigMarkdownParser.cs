//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class MarkdigMarkdownParser : IContentTransformer
    {
        private readonly MarkdownPipeline m_MarkdownEngine;

        public MarkdigMarkdownParser()
        {
            m_MarkdownEngine = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseObservableLinks()
                //.UseSyntaxHighlighting() //requires Markdig.SyntaxHighlighting
                .Build();
        }
        
        public Task<string> Transform(string content, string key, ContextModel model)
        {
            //NOTE: by some reasons extra new line symbol is added to the output
            return Task.Run<string>(() => Markdown.ToHtml(content, m_MarkdownEngine).Trim('\n'));
        }
    }

    public class ObservableLinkInlineRenderer : Markdig.Renderers.Html.Inlines.LinkInlineRenderer 
    {
        protected override void Write(HtmlRenderer renderer, LinkInline link)
        {
            base.Write(renderer, link);
        }
    }

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

    public static class ObservableLinksExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseObservableLinks(this MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.Extensions.Contains<ObservableLinkExtension>())
            {
                pipeline.Extensions.Add(new ObservableLinkExtension());
            }

            return pipeline;
        }
    }
}
