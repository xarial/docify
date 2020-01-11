﻿//*********************************************************************
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
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core.Compiler
{
    public class IncludesHandler : IIncludesHandler
    {
        private const string NAME_PARAMS_SPLIT_SYMBOL = " ";

        private readonly IContentTransformer m_Transformer;

        public IncludesHandler(IContentTransformer transformer) 
        {
            m_Transformer = transformer;
        }
        
        public async Task<string> Insert(string name, Metadata param, 
            Site site, Page page)
        {
            var include = site.Includes.FirstOrDefault(i => string.Equals(i.Name, 
                name, StringComparison.CurrentCultureIgnoreCase));

            if (include == null) 
            {
                throw new MissingIncludeException(name);
            }

            return await m_Transformer.Transform(include.RawContent, include.Key, 
                new IncludeContextModel(site, page, param.Merge(include.Data)));
        }

        public Task ParseParameters(string rawContent, out string name, out Metadata param) 
        {
            rawContent = rawContent.Trim();

            if (rawContent.Contains(NAME_PARAMS_SPLIT_SYMBOL))
            {
                name = rawContent.Substring(0, rawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL));
                var paramStr = rawContent.Substring(rawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL) + 1);

                var yamlDeserializer = new DeserializerBuilder().Build();

                param = yamlDeserializer.Deserialize<Metadata>(paramStr);
            }
            else 
            {
                name = rawContent;
                param = new Metadata();
            }

            return Task.CompletedTask;
        }
    }
}