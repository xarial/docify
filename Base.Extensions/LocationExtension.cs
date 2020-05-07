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
        private const string URL_SEP = "/";
        public const string ID_SEP = "::";

        private const string INDEX_PAGE_NAME = "index.html";

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
                
        public static bool IsRoot(this ILocation loc) => !loc.Path.Any();

        public static string GetRoot(this ILocation loc) => loc.Path.FirstOrDefault();

        public static bool IsFile(this ILocation loc) => !string.IsNullOrEmpty(loc.FileName);
        
        public static ILocation Combine(this ILocation loc, params string[] blocks)
        {
            return loc.Copy("", loc.Path.Union(blocks));
        }

        public static ILocation Combine(this ILocation loc, ILocation other)
        {
            return loc.Copy(other.FileName, loc.Path.Union(other.Path));
        }

        public static ILocation GetParent(this ILocation loc, int level = 1) 
        {
            if (loc.IsFile())
            {
                return loc.Copy("", loc.Path.Take(loc.Path.Count - (level - 1)));
            }
            else 
            {
                return loc.Copy("", loc.Path.Take(loc.Path.Count - level));
            }
        }

        public static bool IsInLocation(this ILocation loc, ILocation parent, 
            StringComparison compType = StringComparison.CurrentCultureIgnoreCase) 
        {
            if (parent.IsFile()) 
            {
                throw new Exception("Parent must not be a file");
            }

            if (loc.Path.Count >= parent.Path.Count) 
            {
                for (int i = 0; i < parent.Path.Count; i++) 
                {
                    if (!string.Equals(loc.Path[i], parent.Path[i], compType))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        public static ILocation GetRelative(this ILocation loc, 
            ILocation relativeTo, StringComparison compType = StringComparison.CurrentCultureIgnoreCase) 
        {
            if (loc.IsInLocation(relativeTo, compType))
            {
                return loc.Copy(loc.FileName, loc.Path.Skip(relativeTo.Path.Count));
            }
            else 
            {
                throw new Exception($"'{loc.ToId()}' location is not within the '{relativeTo.ToId()}'");
            }
        }

        public static bool IsSame(this ILocation loc, ILocation other, 
            StringComparison compType = StringComparison.CurrentCultureIgnoreCase)
        {
            if (ReferenceEquals(loc, other))
            {
                return true;
            }

            if (loc is null || other is null)
            {
                return false;
            }

            if (!string.Equals(loc.FileName, other.FileName, compType)) 
            {
                return false;
            }

            if (loc.Path.Count == other.Path.Count)
            {
                for (int i = 0; i < loc.Path.Count; i++)
                {
                    if (!string.Equals(loc.Path[i], other.Path[i], compType))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
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
