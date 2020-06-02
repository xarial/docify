//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service to cleanup the output directory for <see cref="IPublisher"/> service
    /// </summary>
    public interface ITargetDirectoryCleaner
    {
        /// <summary>
        /// Clear the output directory
        /// </summary>
        /// <param name="outDir">Location of the otput directory</param>
        Task ClearDirectory(ILocation outDir);
    }
}
