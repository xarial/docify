using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Xarial.Docify.Base
{
    public static class LocationExtension
    {
        public const string PATH_SEP = "\\";

        private const string INDEX_PAGE_NAME = "index.html";

        private const string URL_SEP = "/";
        public const string ID_SEP = "::";

        public static string ToPath(this ILocation loc, string root = "")
        {
            return FormFullLocation(loc, root, PATH_SEP);
        }

        public static string ToId(this ILocation loc)
        {
            return FormFullLocation(loc, "", ID_SEP);
        }

        public static string ToUrl(this ILocation loc, string baseUrl = "")
        {
            var url = FormFullLocation(loc, baseUrl, URL_SEP);

            if (url.EndsWith(INDEX_PAGE_NAME, StringComparison.CurrentCultureIgnoreCase))
            {
                if (string.Equals(url, INDEX_PAGE_NAME))
                {
                    url = "";
                }
                else
                {
                    url = url.Substring(0, url.Length - INDEX_PAGE_NAME.Length - 1);
                }

                if (!string.Equals(url, baseUrl, StringComparison.CurrentCultureIgnoreCase))
                {
                    url += "/";
                }
            }

            if (string.IsNullOrEmpty(baseUrl))
            {
                url = "/" + url.TrimStart('/');
            }

            return url;
        }

        public static bool IsEmpty(this ILocation loc) => !loc.Path.Any() && string.IsNullOrEmpty(loc.FileName);
        public static int GetTotalLevel(this ILocation loc) => loc.Path.Count;
        public static bool IsRoot(this ILocation loc)=> !loc.Path.Any();
        public static string GetRoot(this ILocation loc)=> loc.Path.FirstOrDefault();

        public static bool IsIndexPage(this ILocation loc)
        {
            return Path.GetFileNameWithoutExtension(loc.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        public static ILocation ConvertToPageLocation(this ILocation location)
        {
            var fileName = location.FileName;

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = INDEX_PAGE_NAME;
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + ".html";
            }

            return location.Copy(fileName, location.Path.ToArray());
        }

        public static ILocation Combine(this ILocation loc, params string[] blocks)
        {
            return loc.Copy("", loc.Path.Union(blocks));
        }

        private static string FormFullLocation(ILocation loc, string basePart, string sep)
        {
            var fullLoc = new StringBuilder();

            if (!string.IsNullOrEmpty(basePart))
            {
                fullLoc.Append(basePart);
            }

            foreach (var block in loc.Path)
            {
                if (fullLoc.Length > 0)
                {
                    fullLoc.Append(sep);
                }

                fullLoc.Append(block);
            }

            if (!string.IsNullOrEmpty(loc.FileName))
            {
                if (fullLoc.Length > 0)
                {
                    fullLoc.Append(sep);
                }

                fullLoc.Append(loc.FileName);
            }

            return fullLoc.ToString();
        }
    }
}
