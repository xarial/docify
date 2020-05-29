//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
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
