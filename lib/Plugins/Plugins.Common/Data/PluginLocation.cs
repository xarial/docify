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

namespace Xarial.Docify.Lib.Plugins.Common.Data
{
    public class PluginLocation : ILocation
    {
        public static string[] PathSeparators { get; } = new string[] { "\\", "/", "::" };

        public static ILocation FromPath(string path)
        {
            var isRel = !PathSeparators.Any(s => path.StartsWith(s));
            var parts = path.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);

            var offset = 0;
            var fileName = "";

            if (parts.Any() && System.IO.Path.HasExtension(parts.Last()))
            {
                offset = 1;
                fileName = parts.Last();
            }

            var dir = parts.Take(parts.Length - offset);

            if (!isRel)
            {
                dir = new string[] { "" }.Concat(dir);
            }

            return new PluginLocation(fileName, dir);
        }

        public IReadOnlyList<string> Path { get; }

        public string FileName { get; }

        public PluginLocation(string fileName, IEnumerable<string> path)
        {
            FileName = fileName;
            Path = new List<string>(path);
        }

        public ILocation Copy(string fileName, IEnumerable<string> path)
        {
            return new PluginLocation(fileName, path);
        }
    }
}
