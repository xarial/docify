//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Services composes the site from input source files
    /// </summary>
    /// <remarks>This service will group pages, assets, layouts, includes and plugins</remarks>
    public interface IComposer
    {
        /// <summary>
        /// Compose site
        /// </summary>
        /// <param name="files">Source files</param>
        /// <param name="baseUrl">Base url of the site</param>
        /// <returns>Composed site</returns>
        Task<ISite> ComposeSite(IAsyncEnumerable<IFile> files, string baseUrl);
    }
}
