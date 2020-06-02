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
    public interface IPluginInfo
    {
        string Name { get; }
        IAsyncEnumerable<IFile> Files { get; }
    }

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
