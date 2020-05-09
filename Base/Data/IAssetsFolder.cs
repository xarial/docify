using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Data
{
    public interface IAssetsFolder
    {
        string Name { get; }
        List<IAsset> Assets { get; }
        List<IAssetsFolder> Folders { get; }
    }
}
