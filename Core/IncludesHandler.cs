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
        private readonly IContentTransformer m_Transformer;

        public IncludesHandler(IContentTransformer transformer) 
        {
            m_Transformer = transformer;
        }
        
        public async Task<string> Insert(string name, Dictionary<string, dynamic> param, 
            IEnumerable<Template> includes)
        {
            //TODO: find the template
            var include = includes.FirstOrDefault(i => string.Equals(i.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (include == null) 
            {
                //TODO: throw include not found exception
            }

            //TODO: compose the model for the include
            return await m_Transformer.Transform(include.RawContent, include.Key, null);
        }

        public Task ParseParameters(string rawContent, out string name, out Dictionary<string, dynamic> param) 
        {
            name = "";
            param = null;

            return Task.CompletedTask;
        }
    }
}
