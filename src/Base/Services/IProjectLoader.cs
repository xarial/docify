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
    /// Loads the project files
    /// </summary>
    /// <remarks>This service combines files from all available provides (including the input files and libraries) and plugins</remarks>
    public interface IProjectLoader
    {
        /// <summary>
        /// Loads project
        /// </summary>
        /// <param name="locations">Input locations for projects</param>
        /// <returns>Project files</returns>
        IAsyncEnumerable<IFile> Load(ILocation[] locations);
    }
}
