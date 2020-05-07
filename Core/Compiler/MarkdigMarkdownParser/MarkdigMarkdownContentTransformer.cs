//*********************************************************************
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
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class MarkdigMarkdownContentTransformer : IContentTransformer
    {
        private MarkdownPipeline m_MarkdownEngine;
        
        [ImportPlugins]
        private IEnumerable<IRenderUrlPlugin> m_RenderUrlPlugins = null;

        [ImportPlugins]
        private IEnumerable<IRenderImagePlugin> m_RenderImagePlugins = null;

        [ImportPlugins]
        private IEnumerable<IRenderCodeBlockPlugin> m_RenderCodeBlockPlugins = null;

        private MarkdownPipeline MarkdownEngine
        {
            get
            {
                if (m_MarkdownEngine == null)
                {
                    m_MarkdownEngine = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseObservableLinks(m_RenderUrlPlugins, m_RenderImagePlugins)
                        .UseObservableCodeBlocks(m_RenderCodeBlockPlugins)
                        .UseProtectedTags(IncludesHandler.START_TAG, IncludesHandler.END_TAG) //TODO: should be a dependency
                        .Build();
                }

                return m_MarkdownEngine;
            }
        }

        public Task<string> Transform(string content, string key, IContextModel model)
        {
            var context = new MarkdownParserContext();

            var htmlStr = new StringBuilder();

            Markdown.ToHtml(content, new StringWriter(htmlStr),
                MarkdownEngine, context);

            //NOTE: by some reasons extra new line symbol is added to the output
            var html = htmlStr.ToString().Trim('\n');

            return Task.FromResult(html);
        }
    }
}
