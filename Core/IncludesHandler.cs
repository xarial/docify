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
using Xarial.Docify.Core.Exceptions;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core
{
    public class IncludesContextModel : ContextModel 
    {
        public Dictionary<string, dynamic> Parameters { get; }

        public IncludesContextModel(Site site, Page page, Dictionary<string, dynamic> parameters) 
            : base(site, page)
        {
            Parameters = parameters;
        }
    }

    public class IncludesHandler : IIncludesHandler
    {
        private const string NAME_PARAMS_SPLIT_SYMBOL = " ";

        private readonly IContentTransformer m_Transformer;

        public IncludesHandler(IContentTransformer transformer) 
        {
            m_Transformer = transformer;
        }
        
        public async Task<string> Insert(string name, Dictionary<string, dynamic> param, 
            Site site, Page page)
        {
            var include = site.Includes.FirstOrDefault(i => string.Equals(i.Name, 
                name, StringComparison.CurrentCultureIgnoreCase));

            if (include == null) 
            {
                throw new MissingIncludeException(name);
            }

            return await m_Transformer.Transform(include.RawContent, include.Key, 
                new IncludesContextModel(site, page, MergeParameters(param, include)));
        }

        private Dictionary<string, dynamic> MergeParameters(Dictionary<string, dynamic> parameters, Template include) 
        {
            var resParams = new Dictionary<string, dynamic>(include.Data, StringComparer.CurrentCultureIgnoreCase);

            foreach (var param in parameters) 
            {
                if (resParams.ContainsKey(param.Key)) 
                {
                    resParams[param.Key] = param.Value;
                }
                else 
                {
                    resParams.Add(param.Key, param.Value);
                }
            }

            return resParams;
        }

        public Task ParseParameters(string rawContent, out string name, out Dictionary<string, dynamic> param) 
        {
            rawContent = rawContent.Trim();

            if (rawContent.Contains(NAME_PARAMS_SPLIT_SYMBOL))
            {
                name = rawContent.Substring(0, rawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL));
                var paramStr = rawContent.Substring(rawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL) + 1);

                var yamlDeserializer = new DeserializerBuilder().Build();

                param = yamlDeserializer.Deserialize<Dictionary<string, dynamic>>(paramStr);
            }
            else 
            {
                name = rawContent;
                param = new Dictionary<string, dynamic>();
            }

            return Task.CompletedTask;
        }
    }
}
