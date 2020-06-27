//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using RazorLight;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;

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
            var html = Regex.Replace(content, @"^@page(\r\n|\r|\n)", "");

            if (!string.IsNullOrEmpty(html))
            {                
                html = await m_RazorEngine.CompileRenderStringAsync(
                    key, html, model);
            }

            return html;
        }
    }
}
