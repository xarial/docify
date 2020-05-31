//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core
{
    public class Location : ILocation
    {
        public static Location Empty => new Location(Enumerable.Empty<string>());

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
