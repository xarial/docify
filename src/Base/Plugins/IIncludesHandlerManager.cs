//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    /// <summary>
    /// Handler delegate for resolving include in the plugin via <see cref="IIncludesHandlerManager.RegisterCustomIncludeHandler(string, ResolveCustomIncludeDelegate)"/>
    /// </summary>
    /// <param name="data">Metadata of this include</param>
    /// <param name="page">Page where include is resolved</param>
    /// <returns>Resulting text to place in the include placeholder</returns>
    public delegate Task<string> ResolveCustomIncludeDelegate(IMetadata data, IPage page);

    /// <summary>
    /// API service for extending includes
    /// </summary>
    public interface IIncludesHandlerManager
    {
        /// <summary>
        /// Instance of the current includes handler
        /// </summary>
        IIncludesHandler Instance { get; }

        /// <summary>
        /// Register custom handler for an include
        /// </summary>
        /// <param name="includeName">Name of the include (should not conflict with existing includes)</param>
        /// <param name="handler">Handler delegate for resolving the include</param>
        void RegisterCustomIncludeHandler(string includeName, ResolveCustomIncludeDelegate handler);
    }
}
