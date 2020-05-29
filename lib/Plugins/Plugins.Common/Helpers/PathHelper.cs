using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Xarial.Docify.Lib.Plugins.Helpers
{
    public static class PathHelper
    {
        //TODO: move to toolkit
        public static bool Matches(string path, string filter)
        {
            //TODO: combine into single regex
            var regex = (filter.StartsWith("*") ? "" : "^")
                + Regex.Escape(filter).Replace("\\*", ".*").Replace("\\?", ".")
                + (filter.EndsWith("*") ? "" : "$");

            return Regex.IsMatch(path, regex, RegexOptions.IgnoreCase);
        }
    }
}
