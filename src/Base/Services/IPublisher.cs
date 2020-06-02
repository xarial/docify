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
    /// Service publishes the output files
    /// </summary>
    public interface IPublisher
    {
        /// <summary>
        /// Write output files
        /// </summary>
        /// <param name="loc">Output location</param>
        /// <param name="files">Files to publish</param>
        Task Write(ILocation loc, IAsyncEnumerable<IFile> files);
    }
}
