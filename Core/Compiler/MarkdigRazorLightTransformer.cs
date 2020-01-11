//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;

namespace Xarial.Docify.Core.Compiler
{
    public class MarkdigRazorLightTransformer : IContentTransformer
    {
        private readonly MarkdigMarkdownContentTransformer m_MarkdownTransformer;
        private readonly RazorLightContentTransformer m_RazorTransformer;
        private readonly IIncludesHandler m_IncludesHandler;

        public MarkdigRazorLightTransformer(IIncludesHandler includesHandler)
            : this(c => includesHandler)
        {
        }

        public MarkdigRazorLightTransformer(Func<IContentTransformer, IIncludesHandler> includesHandlerFact)
        {
            m_IncludesHandler = includesHandlerFact.Invoke(this);
            m_MarkdownTransformer = new MarkdigMarkdownContentTransformer();
            m_RazorTransformer = new RazorLightContentTransformer();
        }

        public MarkdigRazorLightTransformer() 
        {
            m_MarkdownTransformer = new MarkdigMarkdownContentTransformer();
            m_RazorTransformer = new RazorLightContentTransformer();
        }

        public async Task<string> Transform(string content, string key, IContextModel model)
        {
            //TODO: think of a better way of passing site and page
            var res = content;
            res = await m_RazorTransformer.Transform(res, key, model);
            res = await m_IncludesHandler.ReplaceAll(res, (model as ContextModel).Site.BaseSite, (model as ContextModel).Page.BasePage);
            res = await m_MarkdownTransformer.Transform(res, key, model);

            return res;
        }
    }
}
