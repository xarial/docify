//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base;

namespace Xarial.Docify.Lib.Plugins.Common.Data
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
