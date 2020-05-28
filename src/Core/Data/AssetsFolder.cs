//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
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
