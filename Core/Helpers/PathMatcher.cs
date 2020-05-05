using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xarial.Docify.Core.Helpers
{
    public static class PathMatcher
    {
        public static bool Matches(IEnumerable<string> filters, string path) 
        {
            //TODO: combine into single regex
            var ignoreRegex = filters.Select(
                i => (i.StartsWith("*") ? "" : "^") + Regex.Escape(i).Replace("\\*", ".*").Replace("\\?", ".") + (i.EndsWith("*") ? "" : "$")).ToArray();

            return ignoreRegex.Any(i => Regex.IsMatch(path, i, RegexOptions.IgnoreCase));
        }
    }
}
