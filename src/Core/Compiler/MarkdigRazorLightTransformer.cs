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
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler
{
    public class MarkdigRazorLightTransformer : IContentTransformer
    {
        private readonly MarkdigMarkdownContentTransformer m_MarkdownTransformer;
        private readonly RazorLightContentTransformer m_RazorTransformer;
        
        public MarkdigRazorLightTransformer(ICompilerExtension ext) 
            : this(new MarkdigMarkdownContentTransformer(ext), new RazorLightContentTransformer())
        {
        }

        public MarkdigRazorLightTransformer(MarkdigMarkdownContentTransformer markdown, RazorLightContentTransformer razor)
        {
            m_MarkdownTransformer = markdown;
            m_RazorTransformer = razor;
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
