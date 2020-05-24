//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using RazorLight;
using RazorLight.Razor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Data;
using System.Text.RegularExpressions;

namespace Xarial.Docify.Core.Compiler
{
    public class RazorLightContentTransformer : IDynamicContentTransformer
    {
        private readonly RazorLightEngine m_RazorEngine;

        public RazorLightContentTransformer() 
        {
            m_RazorEngine = new RazorLightEngineBuilder()
                .AddDefaultNamespaces(typeof(Site).Namespace)
                .UseMemoryCachingProvider()
                .UseEmbeddedResourcesProject(typeof(RazorLightContentTransformer))
                .Build();
        }
        
        public async Task<string> Transform(string content, string key, IContextModel model)
        {
            var html = content;

            html = await m_RazorEngine.CompileRenderStringAsync(
                key, html, model);

            return html;
        }
    }
}
