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
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public static class IncludeExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseIncludes(this MarkdownPipelineBuilder pipeline,
            IIncludesHandler paramsParser)
        {
            if (!pipeline.Extensions.Contains<IncludeExtension>())
            {
                pipeline.Extensions.Add(new IncludeExtension(paramsParser));
            }

            return pipeline;
        }
    }
}
