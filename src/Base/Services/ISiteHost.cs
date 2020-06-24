//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service which hosts site
    /// </summary>
    public interface ISiteHost
    {
        /// <summary>
        /// Hosts the site at the specified location
        /// </summary>
        /// <param name="siteLoc">Site location</param>
        /// <param name="hostCalback">Hosting completed callback</param>
        Task Host(ILocation siteLoc, Func<Task> hostCalback);
    }
}
