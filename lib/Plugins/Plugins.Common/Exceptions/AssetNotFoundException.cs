//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Exceptions
{
    public class AssetNotFoundException : Exception
    {
        public AssetNotFoundException(IAssetsFolder dir, string path)
            : base($"Failed to find the asset from path: '{path}' in '{dir.Name}'")
        {
        }
    }
}
