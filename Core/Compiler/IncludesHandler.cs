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
using System.Text.RegularExpressions;
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
        private const string START_TAG = "{%";
        private const string END_TAG = "%}";

        private const string INCLUDE_REGEX = @"(?:" + START_TAG + @")[.\s\S]*?(:?" + END_TAG + ")";

        private const string NAME_PARAMS_SPLIT_SYMBOL = " ";

        private readonly IContentTransformer m_Transformer;

        public IncludesHandler(IContentTransformer transformer) 
        {
            m_Transformer = transformer;
        }
        
        public async Task<string> Render(string name, Metadata param, 
            Site site, Page page)
        {
            var include = site.Includes.FirstOrDefault(i => string.Equals(i.Name, 
                name, StringComparison.CurrentCultureIgnoreCase));

            if (include == null) 
            {
                throw new MissingIncludeException(name);
            }

            Dictionary<string, dynamic> GetData(Metadata data, string name)
            {
                var extrData = data.GetParameterOrDefault<Dictionary<object, object>>(name);

                if (extrData != null)
                {
                    return extrData.ToDictionary(k => k.Key.ToString(), k => (dynamic)k.Value);
                }
                else
                {
                    return null;
                }
            }

            param = param.Merge(GetData(page.Data, name));
            param = param.Merge(GetData(site.Configuration, name));
            param = param.Merge(include.Data);

            var res = await m_Transformer.Transform(include.RawContent, include.Key, 
                new IncludeContextModel(site, page, param));

            return res;
        }

        public Task ParseParameters(string includeRawContent, out string name, out Metadata param) 
        {
            includeRawContent = includeRawContent.Trim();

            if (includeRawContent.Contains(NAME_PARAMS_SPLIT_SYMBOL))
            {
                name = includeRawContent.Substring(0, includeRawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL));
                var paramStr = includeRawContent.Substring(includeRawContent.IndexOf(NAME_PARAMS_SPLIT_SYMBOL) + 1);

                var yamlDeserializer = new DeserializerBuilder().Build();

                param = yamlDeserializer.Deserialize<Metadata>(paramStr);
            }
            else 
            {
                name = includeRawContent;
                param = new Metadata();
            }

            return Task.CompletedTask;
        }

        public Task<string> ReplaceAll(string rawContent, Site site, Page page)
        {
            return ReplaceAsync(rawContent, INCLUDE_REGEX, m => 
            {
                var includeRawContent = m.Value.Substring(START_TAG.Length, m.Value.Length - START_TAG.Length - END_TAG.Length).Trim();
                string name;
                Metadata data;
                ParseParameters(includeRawContent, out name, out data);
                var replace = Render(name, data, site, page);
                return replace;
            }, site, page);
        }

        private async Task<string> ReplaceAsync(string input, string pattern, Func<Match, Task<string>> evaluator, Site site, Page page)
        {
            var hasMatch = false;

            var sb = new StringBuilder();
            var lastIndex = 0;

            var regex = new Regex(pattern);

            foreach (Match match in regex.Matches(input))
            {
                hasMatch = true;
                sb.Append(input, lastIndex, match.Index - lastIndex)
                  .Append(await evaluator.Invoke(match).ConfigureAwait(false));

                lastIndex = match.Index + match.Length;
            }

            sb.Append(input, lastIndex, input.Length - lastIndex);

            var replacement = sb.ToString();

            if (hasMatch)
            {
                return await ReplaceAll(replacement, site, page);
            }
            else 
            {
                return replacement;
            }
        }
    }
}
