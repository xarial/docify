//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Exceptions
{
    public class AssetNotFoundException : BaseUserMessageException
    {
        public AssetNotFoundException(IAssetsFolder dir, string path)
            : base($"Failed to find the asset from path: '{path}' in '{dir.Name}'")
        {
        }
    }
}
