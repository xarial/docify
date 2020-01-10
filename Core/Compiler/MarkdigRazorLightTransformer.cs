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
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;

namespace Xarial.Docify.Core.Compiler
{
    public class MarkdigRazorLightTransformer : IContentTransformer
    {
        private readonly IContentTransformer m_MarkdownTransformer;
        private readonly RazorLightContentTransformer m_RazorTransformer;

        public MarkdigRazorLightTransformer(IIncludesHandler includesHandler) 
            : this(c => includesHandler)
        {
        }

        public MarkdigRazorLightTransformer(Func<IContentTransformer, IIncludesHandler> includesHandlerFact) 
        {
            var includesHandler = includesHandlerFact.Invoke(this);
            m_MarkdownTransformer = new MarkdigMarkdownContentTransformer(includesHandler);
            m_RazorTransformer = new RazorLightContentTransformer();
        }

        public MarkdigRazorLightTransformer(MarkdigMarkdownContentTransformer markDownTransformer, 
            RazorLightContentTransformer razorTransformer)
        {
            m_MarkdownTransformer = markDownTransformer;
            m_RazorTransformer = razorTransformer;
        }

        public async Task<string> Transform(string content, string key, IContextModel model)
        {
            var res = content;

            res = await m_RazorTransformer.Transform(res, key, model);
            res = await m_MarkdownTransformer.Transform(res, key, model);

            return res;
        }
    }
}
