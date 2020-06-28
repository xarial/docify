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
                    "Xarial", "Docify", "Library", "library.manifest"));
        }

        public static Location Empty => new Location("", "", Enumerable.Empty<string>());

        public static Location FromPath(string path)
        {
            LocationExtension.ParsePath(path, out string root, out string fileName, out string[] segs);
            return new Location(root, fileName, segs);
        }

        public static Location FromString(string loc)
        {
            LocationExtension.ParseRelative(loc, out string fileName, out string[] segments);

            return new Location("", fileName, segments);
        }

        public IReadOnlyList<string> Segments { get; }

        public string FileName { get; }

        public string Root { get; }

        public Location(string root, string fileName, IEnumerable<string> path)
        {
            Root = root;
            FileName = fileName;
            Segments = new List<string>(path);
        }
        
        public override string ToString()
        {
            return this.ToId();
        }

        public ILocation Copy(string root, string fileName, IEnumerable<string> path)
        {
            return new Location(root, fileName, path.ToArray());
        }
    }
}
