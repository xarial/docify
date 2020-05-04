﻿using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Lib.Plugins
{
    public class Location : ILocation
    {
        public IReadOnlyList<string> Path { get; }

        public string FileName { get; }

        public Location(string fileName, IEnumerable<string> path) 
        {
            FileName = fileName;
            Path = new List<string>(path);
        }

        public ILocation Copy(string fileName, IEnumerable<string> path)
        {
            return new Location(fileName, path);
        }
    }
}
