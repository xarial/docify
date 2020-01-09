//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class IncludesHandler : IIncludesHandler
    {
        private readonly IEnumerable<Template> m_Includes;
        private readonly IContentTransformer m_Transformer;

        public IncludesHandler(IEnumerable<Template> includes, IContentTransformer transformer) 
        {
            m_Includes = includes;
        }

        public async Task<string> Replace(string content)
        {
            //TODO: find the template
            var include = m_Includes.First();

            //TODO: 
            return await m_Transformer.Transform(include.RawContent, include.Key, null);
        }
    }
}
