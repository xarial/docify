using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class AssetsFolder : IAssetsFolder
    {
        public string Name { get; }
        public List<IAsset> Assets { get; }
        public List<IAssetsFolder> Folders { get; }

        public AssetsFolder(string name) 
        {
            Name = name;
            Assets = new List<IAsset>();
            Folders = new List<IAssetsFolder>();
        }
    }
}
