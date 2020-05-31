//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    public interface IAssetsFolder
    {
        string Name { get; }
        List<IAsset> Assets { get; }
        List<IAssetsFolder> Folders { get; }
    }
}
