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
            LocationExtension.ParsePath(path, out string root, out string fileName, out string[] segments);
            return new PluginLocation(root, fileName, segments);
        }
        
        public string FileName { get; }

        public IReadOnlyList<string> Segments { get; }

        public string Root { get; }

        public PluginLocation(string root, string fileName, IEnumerable<string> path)
        {
            Root = root;
            FileName = fileName;
            Segments = new List<string>(path);
        }

        public ILocation Create(string root, string fileName, IEnumerable<string> path)
        {
            return new PluginLocation(root, fileName, path);
        }

        public override string ToString()
        {
            return this.ToId();
        }
    }
}
