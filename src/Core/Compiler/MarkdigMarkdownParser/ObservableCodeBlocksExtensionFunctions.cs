//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Markdig;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public static class ObservableCodeBlocksExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseObservableCodeBlocks(this MarkdownPipelineBuilder pipeline,
            ICompilerExtension ext)
        {
            pipeline.Extensions.AddIfNotAlready(new ObservableCodeBlockExtension(ext));

            return pipeline;
        }
    }
}
