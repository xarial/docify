﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class MarkdigMarkdownContentTransformer : IContentTransformer
    {
        internal const string CONTEXT_MODEL_PARAM_NAME = "_DocifyContextModel_";

        private readonly MarkdownPipeline m_MarkdownEngine;
        private readonly IIncludesHandler m_IncludesHandler;

        public MarkdigMarkdownContentTransformer(IIncludesHandler includesHandler)
        {
            m_IncludesHandler = includesHandler;

            m_MarkdownEngine = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseObservableLinks()
                .UseIncludes(m_IncludesHandler)
                .Build();
        }

        public Task<string> Transform(string content, string key, IContextModel model)
        {
            var context = new MarkdownParserContext();
            context.Properties.Add(CONTEXT_MODEL_PARAM_NAME, model);

            var htmlStr = new StringBuilder();

            Markdown.ToHtml(content, new StringWriter(htmlStr),
                m_MarkdownEngine, context);

            //NOTE: by some reasons extra new line symbol is added to the output
            var html = htmlStr.ToString().Trim('\n');

            return Task.FromResult(html);
        }
    }
}