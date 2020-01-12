//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public static class ObservableCodeBlocksExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseObservableCodeBlocks(this MarkdownPipelineBuilder pipeline, IEnumerable<IRenderCodeBlockPlugin> plugins)
        {
            pipeline.Extensions.AddIfNotAlready(new ObservableCodeBlockExtension(plugins));

            return pipeline;
        }
    }
}
