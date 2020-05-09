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
    public static class ProtectedTagsExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseProtectedTags(this MarkdownPipelineBuilder pipeline, 
            string startTag, string endTag)
        {
            if (!pipeline.Extensions.Contains<ProtectedTagsExtension>())
            {
                pipeline.Extensions.Add(new ProtectedTagsExtension(startTag, endTag));
            }

            return pipeline;
        }
    }
}
