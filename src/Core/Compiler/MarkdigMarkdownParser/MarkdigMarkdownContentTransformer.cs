//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Markdig;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class MarkdigMarkdownContentTransformer : IStaticContentTransformer
    {
        private MarkdownPipeline m_MarkdownEngine;

        private readonly ICompilerExtension m_Ext;

        //TODO: might need to separate the extension to a specific markdown

        public MarkdigMarkdownContentTransformer(ICompilerExtension ext)
        {
            m_Ext = ext;
        }

        private MarkdownPipeline MarkdownEngine
        {
            get
            {
                if (m_MarkdownEngine == null)
                {
                    m_MarkdownEngine = new MarkdownPipelineBuilder()
                        .UseAdvancedExtensions()
                        .UseObservableLinks(m_Ext)
                        .UseObservableCodeBlocks(m_Ext)
                        .UseProtectedTags(IncludesHandler.START_TAG, IncludesHandler.END_TAG) //TODO: should be a dependency
                        .Build();
                }

                return m_MarkdownEngine;
            }
        }

        public Task<string> Transform(string content)
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
