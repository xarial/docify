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
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class CompositionTransformer : IContentTransformer
    {
        private readonly IContentTransformer[] m_Transformers;

        public CompositionTransformer(params IContentTransformer[] transformers) 
        {
            if (transformers == null) 
            {
                throw new ArgumentNullException(nameof(transformers));
            }

            m_Transformers = transformers;
        }

        public async Task<string> Transform(string content, string key, ContextModel model)
        {
            var res = content;

            foreach (var transformer in m_Transformers) 
            {
                res = await transformer.Transform(res, key, model);
            }

            return res;
        }
    }
}
