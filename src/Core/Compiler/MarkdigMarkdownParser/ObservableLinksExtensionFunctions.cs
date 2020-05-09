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
    public static class ObservableLinksExtensionFunctions
    {
        public static MarkdownPipelineBuilder UseObservableLinks(this MarkdownPipelineBuilder pipeline,
            IEnumerable<IRenderUrlPlugin> urlPlugins,
            IEnumerable<IRenderImagePlugin> imagePlugins)
        {
            pipeline.Extensions.AddIfNotAlready(
                new ObservableLinkExtension(urlPlugins, imagePlugins));

            return pipeline;
        }
    }
}
