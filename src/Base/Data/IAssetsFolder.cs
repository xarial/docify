//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
