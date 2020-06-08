//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Xarial.Docify.Base
{
    /// <summary>
    /// Extension methods for <see cref="ILocation"/>
    /// </summary>
    public static class LocationExtension
    {
        public const string PATH_SEP = "\\";
        public const string URL_SEP = "/";
        public const string ID_SEP = "::";

        public const string URL_PROTOCOL_REGEX = "^(http|https):/";

        public const string NEGATIVE_FILTER = "|";
        public const string ANY_FILTER = "*";

        private const string INDEX_PAGE_NAME = "index.html";

        /// <summary>
        /// Converts this location to path
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="root">Root directory</param>
        /// <returns>Path</returns>
        public static string ToPath(this ILocation loc, string root = "")
        {
            return FormFullLocation(loc, root, PATH_SEP);
        }

        /// <summary>
        /// Converts this location to universal id
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>Id of the location</returns>
        public static string ToId(this ILocation loc)
        {
            return FormFullLocation(loc, "", ID_SEP);
        }

        /// <summary>
        /// Converts location to url
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="baseUrl">Base url</param>
        /// <returns>Url</returns>
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

            if (string.IsNullOrEmpty(baseUrl) && 
                (loc.GetRoot() == null || !Regex.IsMatch(loc.GetRoot(), URL_PROTOCOL_REGEX)))
            {
                url = "/" + url.TrimStart('/');
            }

            return url;
        }

        /// <summary>
        /// Checks if location is empty
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>True if location is empty, False if not</returns>
        public static bool IsEmpty(this ILocation loc) => !loc.Path.Any() && string.IsNullOrEmpty(loc.FileName);

        /// <summary>
        /// Checks if this file is a root file
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>True if files is root file, False if it is contained in the sub-folder(s)</returns>
        public static bool IsRoot(this ILocation loc) => !loc.Path.Any();

        /// <summary>
        /// Gets the root name of this location
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>Root of the location</returns>
        public static string GetRoot(this ILocation loc) => loc.Path.FirstOrDefault();

        /// <summary>
        /// Checks if this location is file or folder
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>True if location is file, False if folder</returns>
        public static bool IsFile(this ILocation loc) => !string.IsNullOrEmpty(loc.FileName);

        /// <summary>
        /// Combines location with aditional data
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="blocks">Blocks to append to location</param>
        /// <returns>New combined location</returns>
        public static ILocation Combine(this ILocation loc, params string[] blocks)
        {
            return loc.Copy("", loc.Path.Union(blocks));
        }

        /// <see cref="Combine(ILocation, string[])"/>
        /// <param name="other">Other location to append to this location</param>
        public static ILocation Combine(this ILocation loc, ILocation other)
        {
            return loc.Copy(other.FileName, loc.Path.Union(other.Path));
        }

        /// <summary>
        /// Gets the parent folder of this location
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="level">Parent level</param>
        /// <returns>Parent location</returns>
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

        /// <summary>
        /// Checks if this location is within another location
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="parent">Location to check agains</param>
        /// <param name="compType">Comparison type</param>
        /// <returns>True if location is within the parent location, False if not</returns>
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

        /// <summary>
        /// Finds the relative location
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="relativeTo">Location to get relative to</param>
        /// <param name="compType">Comparison type</param>
        /// <returns>Relative location</returns>
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

        /// <summary>
        /// Compares two locations
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="other">Other location to compare</param>
        /// <param name="compType">Comparison type</param>
        /// <returns>True if locations are the same, False if different</returns>
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
                if (!string.IsNullOrEmpty(loc.FileName) || !string.IsNullOrEmpty(other.FileName))
                {
                    return false;
                }
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

        /// <summary>
        /// Checks if location matches the specified filters
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="filters">Filters to match</param>
        /// <returns>True if matches, False if not</returns>
        public static bool Matches(this ILocation loc, IEnumerable<string> filters)
        {
            if (filters?.Any() != true)
            {
                return true;
            }
            else
            {
                filters = filters.Select(f => f
                    .Replace(PATH_SEP, ID_SEP)
                    .Replace(URL_SEP, ID_SEP)).ToArray();
            }

            var path = loc.ToId();

            //TODO: combine into single regex

            bool MatchFilter(string regexFilter)
            {
                var regex = (regexFilter.StartsWith(ANY_FILTER) ? "" : "^")
                    + Regex.Escape(regexFilter)
                    .Replace($"\\{ANY_FILTER}", ".*")
                    .Replace("\\?", ".")
                    + (regexFilter.EndsWith(ANY_FILTER) ? "" : "$");

                return Regex.IsMatch(path, regex, RegexOptions.IgnoreCase);
            }

            var posFilters = filters.Where(f => !IsNegative(f));
            var negFilters = filters.Except(posFilters);

            if (posFilters.Any() && !posFilters.Any(f => MatchFilter(f)))
            {
                return false;
            }

            if (negFilters.Any(f => MatchFilter(RevertFilter(f))))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Reverses the filter
        /// </summary>
        /// <param name="filter">Filter to reverse</param>
        /// <returns>Returns negative filter for positive input and vice-versa</returns>
        /// <remarks>Use the filters with <see cref="LocationExtension.Matches(ILocation, IEnumerable{string})"/> method</remarks>
        public static string RevertFilter(string filter)
        {
            if (IsNegative(filter))
            {
                return filter.Substring(1);
            }
            else
            {
                return NEGATIVE_FILTER + filter;
            }
        }

        private static bool IsNegative(string filter) => filter.StartsWith(NEGATIVE_FILTER);

        private static string FormFullLocation(ILocation loc, string basePart, string sep)
        {
            var fullLoc = new StringBuilder();

            var hasContent = false;

            if (!string.IsNullOrEmpty(basePart))
            {
                fullLoc.Append(basePart);
                hasContent = true;
            }

            foreach (var block in loc.Path)
            {
                if (hasContent)
                {
                    fullLoc.Append(sep);
                }

                fullLoc.Append(block);

                hasContent = true;
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
