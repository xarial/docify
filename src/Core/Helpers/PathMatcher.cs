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
    public static class PathMatcher
    {
        public static bool Matches(IEnumerable<string> filters, string path) 
        {
            //TODO: combine into single regex
            var regex = filters.Select(
                i => (i.StartsWith("*") ? "" : "^") + Regex.Escape(i).Replace("\\*", ".*").Replace("\\?", ".") + (i.EndsWith("*") ? "" : "$")).ToArray();

            return regex.Any(i => Regex.IsMatch(path, i, RegexOptions.IgnoreCase));
        }
    }
}
