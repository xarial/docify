//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Represents a collection of <see cref="IAsset"/>
    /// </summary>
    public interface IAssetsFolder
    {
        /// <summary>
        /// Name of the folder
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Top level assets in this folder
        /// </summary>
        List<IAsset> Assets { get; }

        /// <summary>
        /// Sub folders
        /// </summary>
        List<IAssetsFolder> Folders { get; }
    }
}
