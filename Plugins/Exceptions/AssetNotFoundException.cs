using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Exceptions
{
    public class AssetNotFoundException : Exception
    {
        public AssetNotFoundException(IAssetsFolder dir, string path) 
            : base($"Failed to find the asset from path: '{path}' in '{dir.Name}'") 
        {
        }
    }
}
