//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using Markdig.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class MarkdigMarkdownParser : IMarkdownParser
    {
        private readonly MarkdownPipeline m_MarkdownEngine;

        public MarkdigMarkdownParser() 
        {
            m_MarkdownEngine = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                //.UseSyntaxHighlighting() //requires Markdig.SyntaxHighlighting
                .Build();
        }

        public string Parse(string content)
        {
            //NOTE: by some reasons extra new line symbol is added to the output
            return Markdown.ToHtml(content, m_MarkdownEngine).Trim('\n');
        }
    }
}
