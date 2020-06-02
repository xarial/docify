//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base
{
    /// <summary>
    /// Represents the universal location
    /// </summary>
    /// <remarks>This is used to be able to combine different formats of paths and urls into a single element</remarks>
    public interface ILocation
    {
        /// <summary>
        /// Breadcrumb path
        /// </summary>
        IReadOnlyList<string> Path { get; }

        /// <summary>
        /// Name of the file or an empty string for a folder
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Initiates an instance from the input data
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="path">Path</param>
        /// <returns>Instance of this location</returns>
        ILocation Copy(string fileName, IEnumerable<string> path);
    }
}
