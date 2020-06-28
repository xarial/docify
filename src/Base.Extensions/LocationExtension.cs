﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
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
        //public static readonly char PATH_SEP = System.IO.Path.DirectorySeparatorChar;
        public const char URL_SEP = '/';
        public const string ID_SEP = "::";

        //public const string URL_PROTOCOL_REGEX = "^(http|https):/";

        public const string NEGATIVE_FILTER = "|";
        public const string ANY_FILTER = "*";

        private const string INDEX_PAGE_NAME = "index.html";

        /// <summary>
        /// Converts this location to path
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="root">Root directory</param>
        /// <returns>Path</returns>
        public static string ToPath(this ILocation loc) => Path.Combine(GetAllSegments(loc).ToArray());
        
        /// <summary>
        /// Converts this location to universal id
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>Id of the location</returns>
        public static string ToId(this ILocation loc) => string.Join(ID_SEP, GetAllSegments(loc));

        /// <summary>
        /// Converts location to url
        /// </summary>
        /// <param name="loc">Location</param>
        /// <param name="baseUrl">Base url</param>
        /// <returns>Url</returns>
        public static string ToUrl(this ILocation loc)
        {
            var url = string.Join(URL_SEP, GetAllSegments(loc));

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

                url += "/";
            }

            if (string.IsNullOrEmpty(loc.Root))
            {
                url = "/" + url.TrimStart('/');
            }

            return url;
        }

        private static IEnumerable<string> GetAllSegments(ILocation loc) 
        {
            if (!string.IsNullOrEmpty(loc.Root)) 
            {
                yield return loc.Root;
            }

            foreach (var seg in loc.Segments) 
            {
                yield return seg;
            }

            if (!string.IsNullOrEmpty(loc.FileName)) 
            {
                yield return loc.FileName;
            }
        }

        /// <summary>
        /// Checks if location is empty
        /// </summary>
        /// <param name="loc">Location</param>
        /// <returns>True if location is empty, False if not</returns>
        public static bool IsEmpty(this ILocation loc) => !loc.Segments.Any() && string.IsNullOrEmpty(loc.FileName);

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
            return loc.Copy(loc.Root, "", loc.Segments.Concat(blocks));
        }

        /// <see cref="Combine(ILocation, string[])"/>
        /// <param name="other">Other location to append to this location</param>
        public static ILocation Combine(this ILocation loc, ILocation other)
        {
            return loc.Copy(loc.Root, other.FileName, loc.Segments.Concat(other.Segments));
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
                return loc.Copy(loc.Root, "", loc.Segments.Take(loc.Segments.Count - (level - 1)));
            }
            else
            {
                return loc.Copy(loc.Root, "", loc.Segments.Take(loc.Segments.Count - level));
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

            if (string.Equals(loc.Root, parent.Root, StringComparison.CurrentCultureIgnoreCase)
                || string.IsNullOrEmpty(loc.Root) && string.IsNullOrEmpty(parent.Root))
            {
                if (loc.Segments.Count >= parent.Segments.Count)
                {
                    for (int i = 0; i < parent.Segments.Count; i++)
                    {
                        if (!string.Equals(loc.Segments[i], parent.Segments[i], compType))
                        {
                            return false;
                        }
                    }

                    return true;
                }
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
                return loc.Copy(loc.Root, loc.FileName, loc.Segments.Skip(relativeTo.Segments.Count));
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

            if (loc.Segments.Count == other.Segments.Count)
            {
                for (int i = 0; i < loc.Segments.Count; i++)
                {
                    if (!string.Equals(loc.Segments[i], other.Segments[i], compType))
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
            if (!string.IsNullOrEmpty(loc.Root)) 
            {
                throw new Exception("Only relative paths are supported");
            }

            if (filters?.Any() != true)
            {
                return true;
            }
            else
            {
                filters = filters.Select(f => f
                    .Replace(Path.DirectorySeparatorChar.ToString(), ID_SEP)
                    .Replace(URL_SEP.ToString(), ID_SEP)).ToArray();
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

        public static void ParsePath(string path, out string root, 
            out string fileName, out string[] segments)
        {
            IEnumerable<string> GetSegments(string path)
            {
                while (!string.IsNullOrEmpty(path))
                {
                    var seg = "";

                    var prevPath = path;
                    path = Path.GetDirectoryName(path);
                    if (!string.IsNullOrEmpty(path))
                    {
                        seg = Path.GetRelativePath(path, prevPath);
                    }
                    else
                    {
                        seg = prevPath;
                    }

                    if (seg != ".")
                    {
                        yield return seg;
                    }
                }
            }
                        
            fileName = Path.GetFileName(path);
            if (string.IsNullOrEmpty(Path.GetExtension(fileName)))
            {
                fileName = "";
            }
            
            if (!string.IsNullOrEmpty(fileName))
            {
                path = Path.GetDirectoryName(path);
            }

            root = Path.GetPathRoot(path);
            
            if (!string.IsNullOrEmpty(root))
            {
                path = Path.GetRelativePath(root, path);
            }

            segments = GetSegments(path).Reverse().ToArray();
        }

        public static void ParseRelative(string path,
            out string fileName, out string[] segments)
        {
            segments = path.Split(new string[]
            {
                Path.DirectorySeparatorChar.ToString(),
                URL_SEP.ToString(),
                ID_SEP
            }, StringSplitOptions.None).ToArray();

            fileName = "";

            if (!string.IsNullOrEmpty(Path.GetExtension(segments.Last())))
            {
                fileName = segments.Last();
                segments = segments.Take(segments.Length - 1).ToArray();
            }
        }
    }
}
