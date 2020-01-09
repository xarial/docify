//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using RazorLight;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core
{
    public class RazorLightEvaluator : IContentTransformer
    {
        private readonly RazorLightEngine m_RazorEngine;

        public RazorLightEvaluator() 
        {
            m_RazorEngine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();
        }
        
        public async Task<string> Transform(string content, string key, IContextModel model)
        {
            var html = content;

            if (HasRazorCode(content))
            {
                html = await m_RazorEngine.CompileRenderAsync(
                    key, html, model, model?.GetType());
            }

            return html;
        }

        private bool HasRazorCode(string content)
        {
            //TODO: might need to have better logic to identify this
            return content?.Contains('@') == true;
        }
    }
}
