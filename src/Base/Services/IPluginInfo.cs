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
    /// Plugin information
    /// </summary>
    public interface IPluginInfo
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Plugin files
        /// </summary>
        IAsyncEnumerable<IFile> Files { get; }
    }
}
