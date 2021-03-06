﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service which loads and manages plugins
    /// </summary>
    public interface IPluginsManager
    {
        /// <summary>
        /// Loads plugins from the input files
        /// </summary>
        /// <param name="files">Plugin files</param>
        Task LoadPlugins(IAsyncEnumerable<IPluginInfo> pluginInfos);
    }
}
