//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service loads files from the specified location
    /// </summary>
    public interface IFileLoader
    {
        /// <summary>
        /// Checks if the specifying location exists
        /// </summary>
        /// <param name="location">Location to check</param>
        /// <returns>True if location exists, False if not</returns>
        bool Exists(ILocation location);

        /// <summary>
        /// Loads files from the folder
        /// </summary>
        /// <param name="location">Folder location</param>
        /// <param name="filters">Filters for the files</param>
        /// <returns>Folder content files</returns>
        IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters);
    }
}
