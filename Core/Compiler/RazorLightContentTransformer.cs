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
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Compiler
{
    public class RazorLightContentTransformer : IContentTransformer
    {
        private readonly RazorLightEngine m_RazorEngine;

        public RazorLightContentTransformer() 
        {
            m_RazorEngine = new RazorLightEngineBuilder()
                .AddDefaultNamespaces(typeof(Site).Namespace)
                .UseMemoryCachingProvider()
                .UseEmbeddedResourcesProject(typeof(Site))
                .Build();
        }
        
        public async Task<string> Transform(string content, string key, IContextModel model)
        {
            if (HasRazorCode(content))
            {
                var html = content;

                html = await m_RazorEngine.CompileRenderStringAsync(
                    key, html, model);

                return html;
            }
            else 
            {
                return content;
            }
        }

        //private string GetRazorPageContent(string content, Type modelType)
        //{
        //    return $"@model {modelType.FullName}\r\n" + content;
        //}

        private bool HasRazorCode(string content)
        {
            //TODO: might need to have better logic to identify this
            return content?.Contains('@') == true;
        }
    }
}
