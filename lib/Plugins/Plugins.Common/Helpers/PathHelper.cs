//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Text.RegularExpressions;

namespace Xarial.Docify.Lib.Plugins.Common.Helpers
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
