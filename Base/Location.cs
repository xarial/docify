//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;

namespace Xarial.Docify.Base
{
    public class Location
    {
        public static Location FromPath(string path, string relTo = "")
        {
            //TODO: check if path is valid

            if (!string.IsNullOrEmpty(relTo))
            {
                if (path.StartsWith(relTo, StringComparison.CurrentCultureIgnoreCase))
                {
                    path = path.Substring(relTo.Length).TrimStart('\\');
                }
            }

            var isFile = !string.IsNullOrEmpty(System.IO.Path.GetExtension(path));

            var fileName = System.IO.Path.GetFileName(path);
            
            var dir = System.IO.Path.GetDirectoryName(path);

            if (!isFile)
            {
                fileName = "";
                dir = path;
            }

            string[] blocks = null;

            if (!string.IsNullOrEmpty(dir))
            {
                blocks = dir.Split(LocationExtension.PATH_SEP).ToArray();
            }
            else
            {
                blocks = new string[0];
            }

            return new Location(fileName, blocks);
        }

        public bool IsEmpty
        {
            get
            {
                return !Path.Any() && string.IsNullOrEmpty(FileName);
            }
        }

        public IReadOnlyList<string> Path { get; }

        public int TotalLevel
        {
            get
            {
                return Path.Count;
            }
        }

        public bool IsRoot
        {
            get
            {
                return !Path.Any();
            }
        }

        public string Root
        {
            get
            {
                return Path.FirstOrDefault();
            }
        }

        public string FileName { get; }

        public Location(string fileName, params string[] path)
        {
            FileName = fileName;
            Path = new List<string>(path);
        }

        public Location(IEnumerable<string> path)
        {
            Path = new List<string>(path);
        }

        public override string ToString()
        {
            return this.ToId();
        }
    }

    public static class LocationExtension
    {
        internal const char PATH_SEP = '\\';
        private const char URL_SEP = '/';
        private const char ID_SEP = '-';

        public static string ToPath(this Location loc, string root = "")
        {
            return FormFullLocation(loc, root, PATH_SEP);
        }

        public static string ToId(this Location loc)
        {
            return FormFullLocation(loc, "", ID_SEP);
        }

        public static string ToUrl(this Location loc, string baseUrl = "")
        {
            return FormFullLocation(loc, baseUrl, URL_SEP);
        }

        public static Location Combine(this Location loc, params string[] blocks) 
        {
            return new Location(loc.Path.Union(blocks));
        }

        private static string FormFullLocation(Location loc, string basePart, char sep)
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
