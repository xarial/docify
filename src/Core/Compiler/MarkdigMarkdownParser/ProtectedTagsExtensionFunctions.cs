//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Markdig;

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
