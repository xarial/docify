using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Data
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
