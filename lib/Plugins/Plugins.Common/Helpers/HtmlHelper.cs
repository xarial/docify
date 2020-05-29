using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Xarial.Docify.Lib.Plugins.Helpers
{
    public static class HtmlHelper
    {
        public static string HtmlToPlainText(HtmlNode mainNode)
        {
            var ignoreNodes = new string[]
            {
                "script", "style"
            };

            var res = new StringBuilder();

            foreach (var textNode in mainNode.SelectNodes(".//text()")
                .Where(n => !ignoreNodes.Contains(n.ParentNode.Name, StringComparer.CurrentCultureIgnoreCase)))
            {
                var txt = textNode.InnerText;

                if (!string.IsNullOrEmpty(txt))
                {
                    if (!Regex.IsMatch(txt, "^(\\n|\\r|\\r\\n) +$"))
                    {
                        res.Append(WebUtility.HtmlDecode(txt));
                    }
                }
            }

            return res.ToString();
        }
    }
}
