//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core
{
    public class Location : ILocation
    {
        public static class Library
        {
            public const string ComponentsFolderName = "_components";
            public const string ThemesFolderName = "_themes";
            public const string PluginsFolderName = "_plugins";

            public static ILocation DefaultLibraryManifestFilePath 
                => FromPath(System.IO.Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                    "Xarial\\Docify\\Library\\library.manifest"));
        }

        public static Location Empty => new Location(Enumerable.Empty<string>());

        //TODO: this needs better approach
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

            string[] blocks;

            if (!string.IsNullOrEmpty(dir))
            {
                blocks = dir.Split(new string[] { LocationExtension.PATH_SEP }, StringSplitOptions.None).ToArray();
            }
            else
            {
                blocks = new string[0];
            }

            return new Location(fileName, blocks);
        }

        //TODO: this needs better approach
        public static Location FromUrl(string url) 
        {
            string protocol = null;

            url = Regex.Replace(url, LocationExtension.URL_PROTOCOL_REGEX, m =>
            {
                protocol = m.Value;
                return "";
            });

            var parts = url.Split(LocationExtension.URL_SEP, StringSplitOptions.RemoveEmptyEntries);

            if (!string.IsNullOrEmpty(protocol)) 
            {
                parts = new string[] { protocol }.Union(parts).ToArray();
            }

            var fileName = "";

            if (System.IO.Path.HasExtension(parts.Last())) 
            {
                fileName = parts.Last();
                parts = parts.Take(parts.Length - 1).ToArray();
            }

            return new Location(fileName, parts);
        }

        public static Location FromString(string loc)
        {
            var blocks = loc.Split(new string[] 
            {
                LocationExtension.PATH_SEP,
                LocationExtension.URL_SEP, 
                LocationExtension.ID_SEP 
            }, StringSplitOptions.None).ToArray();

            var fileName = "";

            if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(blocks.Last()))) 
            {
                fileName = blocks.Last();
                blocks = blocks.Take(blocks.Length - 1).ToArray();
            }
            
            return new Location(fileName, blocks);
        }

        public IReadOnlyList<string> Path { get; }

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

        public ILocation Copy(string fileName, IEnumerable<string> path)
        {
            return new Location(fileName, path.ToArray());
        }
    }
}
