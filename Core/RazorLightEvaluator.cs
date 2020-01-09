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
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class RazorLightEvaluator : IScriptEvaluator
    {
        private readonly RazorLightEngine m_RazorEngine;

        public RazorLightEvaluator() 
        {
            m_RazorEngine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> Evaluate(ICompilable compilable, ContextModel model)
        {
            var html = compilable.RawContent;

            if (HasRazorCode(compilable))
            {
                html = await m_RazorEngine.CompileRenderAsync(
                    compilable.Key, html, model, model?.GetType());
            }

            return html;
        }

        private bool HasRazorCode(ICompilable page)
        {
            //TODO: might need to have better logic to identify this
            return page.RawContent?.Contains('@') == true;
        }
    }
}
