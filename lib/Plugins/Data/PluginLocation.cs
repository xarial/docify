﻿using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Lib.Plugins.Data
{
    public class PluginLocation : ILocation
    {
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