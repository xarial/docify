//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Data
{
    public class PluginAssetsFolder : IAssetsFolder
    {
        public string Name { get; }

        public List<IAsset> Assets { get; }

        public List<IAssetsFolder> Folders { get; }

        public PluginAssetsFolder(string name)
        {
            Name = name;
            Assets = new List<IAsset>();
            Folders = new List<IAssetsFolder>();
        }
    }
}
