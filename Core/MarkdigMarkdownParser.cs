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
using Markdig.Renderers.Html;
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
        private readonly IIncludesHandler m_IncludesHandler;
        
        public MarkdigMarkdownParser(IIncludesHandler includesHandler)
        {
            m_IncludesHandler = includesHandler;

            m_MarkdownEngine = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseObservableLinks()
                .UseIncludes(m_IncludesHandler)
                //.UseSyntaxHighlighting() //requires Markdig.SyntaxHighlighting
                .Build();
        }
        
        public Task<string> Transform(string content, string key, ContextModel model)
        {
            //NOTE: by some reasons extra new line symbol is added to the output
            var html = Markdown.ToHtml(content, m_MarkdownEngine).Trim('\n');

            return Task.FromResult(html);
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
            pipeline.Extensions.AddIfNotAlready<ObservableLinkExtension>();

            return pipeline;
        }
    }

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
            //if (!pipeline.InlineParsers.Contains<JiraLinkInlineParser>())
            //{
            //    // Insert the parser before the link inline parser
            //    pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new JiraLinkInlineParser());
            //}
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

    public static class IncludeExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseIncludes(this MarkdownPipelineBuilder pipeline, IIncludesHandler paramsParser)
        {
            if (!pipeline.Extensions.Contains<IncludeExtension>())
            {
                pipeline.Extensions.Add(new IncludeExtension(paramsParser));
            }

            return pipeline;
        }
    }

    public class IncludeInlineParser : InlineParser
    {
        private readonly IIncludesHandler m_ParamsParser;

        private const string START_TAG = "{%";
        private const string END_TAG = "%}";

        public IncludeInlineParser(IIncludesHandler paramsParser)
        {
            m_ParamsParser = paramsParser;
            OpeningCharacters = new char[] { START_TAG[0] };
        }
        
        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (!slice.Match(START_TAG))
            {
                return false;
            }
            
            var rawContent = new StringBuilder();

            slice.Start = slice.Start + START_TAG.Length - 1;

            var current = slice.NextChar();
            
            while (current != '%' && slice.PeekChar(1) != '}')
            {
                if (slice.IsEmpty) 
                {
                    //TODO: throw exception
                }

                rawContent.Append(current);
                current = slice.NextChar();
            }

            slice.Start = slice.Start + END_TAG.Length;

            string name;
            Dictionary<string, dynamic> param;
            m_ParamsParser.ParseParameters(rawContent.ToString(), out name, out param);

            processor.Inline = new IncludeData(name, param);

            //processor.Inline.Span.End = processor.Inline.Span.Start;

            return true;
        }
    }

    public class IncludeData : LeafInline
    {
        public string Name { get; }
        public Dictionary<string, dynamic> Parameters { get; }

        public IncludeData(string name, Dictionary<string, dynamic> parameters) 
        {
            Name = name;
            Parameters = parameters;
        }
    }

    //public interface IIncludeParameterParser 
    //{
    //    void Parse(string rawContent, out string name, out Dictionary<string, dynamic> param);
    //}

    //public class IncludeParameterParser : IIncludeParameterParser
    //{
    //    public void Parse(string rawContent, out string name, out Dictionary<string, dynamic> param)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class IncludeRenderer : HtmlObjectRenderer<IncludeData>
    {
        private readonly IIncludesHandler m_IncludesHandler;

        public IncludeRenderer(IIncludesHandler includesHandler)
        {
            m_IncludesHandler = includesHandler;
        }

        protected override void Write(HtmlRenderer renderer, IncludeData data)
        {
            if (renderer.EnableHtmlForInline)
            {
                var res = m_IncludesHandler.Insert(data.Name, data.Parameters, null).Result;

                //TODO: merge parameters
                renderer.Write(res);
            }
            else
            {
                //TODO: implement plain writing
            }
        }
    }
}
