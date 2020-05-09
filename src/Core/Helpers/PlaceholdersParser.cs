//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xarial.Docify.Core.Helpers
{
    internal class PlaceholdersParser
    {
        //TODO: implement handling of protected symbols
        private const string REGEX_TEMPLATE = @"(?:{0})[.\s\S]*?(:?{1})";

        private readonly string m_RegexPattern;

        private readonly string m_StartTag;
        private readonly string m_EndTag;

        internal PlaceholdersParser(string startTag, string endTag) 
        {
            m_RegexPattern = string.Format(REGEX_TEMPLATE, Regex.Escape(startTag), Regex.Escape(endTag));

            m_StartTag = startTag;
            m_EndTag = endTag;
        }

        internal async Task<string> ReplaceAsync(string input, Func<string, Task<string>> evaluator)
        {
            var sb = new StringBuilder();
            var lastIndex = 0;

            var regex = new Regex(m_RegexPattern);

            foreach (Match match in regex.Matches(input))
            {
                //TODO: implement group instead of trim
                var plc = match.Value.Substring(m_StartTag.Length, match.Value.Length - m_StartTag.Length - m_EndTag.Length).Trim();

                sb.Append(input, lastIndex, match.Index - lastIndex)
                  .Append(await evaluator.Invoke(plc).ConfigureAwait(false));

                lastIndex = match.Index + match.Length;
            }

            sb.Append(input, lastIndex, input.Length - lastIndex);

            return sb.ToString();
        }
    }
}
