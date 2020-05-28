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

namespace Xarial.Docify.Core.Helpers
{
    //TODO: move to toolkit
    public static class PathMatcher
    {
        public const string NEGATIVE_FILTER = "|";
        public const string ANY_FILTER = "*";

        public static IEnumerable<string> RevertFilter(params string[] filters) 
        {
            foreach (var filter in filters) 
            {
                if (IsNegative(filter))
                {
                    yield return filter.Substring(1);
                }
                else
                {
                    yield return NEGATIVE_FILTER + filter;
                }
            }
        }

        private static bool IsNegative(string filter) => filter.StartsWith(NEGATIVE_FILTER);

        public static bool Matches(IEnumerable<string> filters, string path) 
        {
            //TODO: combine into single regex

            if (filters == null) 
            {
                return true;
            }

            foreach (var filter in filters.ToArray()) 
            {
                var isNegative = IsNegative(filter);

                string regexFilter;

                if (isNegative)
                {
                    regexFilter = RevertFilter(filter).First();
                }
                else 
                {
                    regexFilter = filter;
                }

                var regex = (regexFilter.StartsWith(ANY_FILTER) ? "" : "^") 
                    + Regex.Escape(regexFilter)
                    .Replace($"\\{ANY_FILTER}", ".*")
                    .Replace("\\?", ".") 
                    + (regexFilter.EndsWith(ANY_FILTER) ? "" : "$");

                var match = Regex.IsMatch(path, regex, RegexOptions.IgnoreCase);

                if (match == !isNegative) 
                {
                    return true;
                }
            }

            return false;
        }
    }
}
