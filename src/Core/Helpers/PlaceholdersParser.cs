//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Xarial.Docify.Core.Helpers
{
    internal class PlaceholdersParser
    {
        private const string REGEX_TEMPLATE = @"{0}*({1}([.\s\S]*?){2})";
        private const string REGEX_ESCAPED = @"^{0}{{1}}[^{0}]";

        private readonly string m_RegexPattern;
        private readonly string m_RegexEscapePattern;

        internal PlaceholdersParser(string startTag, string endTag, string protectionSymbol = "\\")
        {
            m_RegexPattern = string.Format(REGEX_TEMPLATE, Regex.Escape(protectionSymbol), Regex.Escape(startTag), Regex.Escape(endTag));
            m_RegexEscapePattern = string.Format(REGEX_ESCAPED, Regex.Escape(protectionSymbol));
        }

        internal async Task<string> ReplaceAsync(string input, Func<string, Task<string>> evaluator)
        {
            var sb = new StringBuilder();
            var lastIndex = 0;

            var regex = new Regex(m_RegexPattern);

            foreach (Match match in regex.Matches(input))
            {
                var includeContent = match.Groups[1];

                if (Regex.IsMatch(match.Value, m_RegexEscapePattern))
                {
                    sb.Append(input, lastIndex, match.Index - lastIndex)
                      .Append(includeContent.Value);
                }
                else 
                {
                    var plc = match.Groups[2].Value.Trim();

                    sb.Append(input, lastIndex, includeContent.Index - lastIndex)
                      .Append(await evaluator.Invoke(plc).ConfigureAwait(false));
                }

                lastIndex = match.Index + match.Length;
            }

            sb.Append(input, lastIndex, input.Length - lastIndex);

            return sb.ToString();
        }
    }
}
