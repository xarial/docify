//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Context;
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
    /// Delegate of <see cref="IIncludesHandlerManager.PreResolveInclude"/> event
    /// </summary>
    /// <param name="includeName">Name of the include to be resolved</param>
    /// <param name="model">Include data model</param>
    public delegate Task PreResolveIncludeDelegate(string includeName, IContextModel model);

    /// <summary>
    /// Delegate of <see cref="IIncludesHandlerManager.PostResolveInclude"/> event
    /// </summary>
    /// <param name="name">Name of the include to be resolved</param>
    /// <param name="model">Include data model</param>
    /// <param name="html">HTML result of the include</param>
    public delegate Task PostResolveIncludeDelegate(string includeName, IContextModel model, StringBuilder html);

    /// <summary>
    /// API service for extending includes
    /// </summary>
    public interface IIncludesHandlerManager
    {
        /// <summary>
        /// Event fired when specific include is about to be resolved
        /// </summary>
        /// <remarks>Use this event to modify the metadata of the include if needed</remarks>
        event PreResolveIncludeDelegate PreResolveInclude;

        /// <summary>
        /// Event fired when include is rendered
        /// </summary>
        /// <remarks>Use this event to alter the html result of include</remarks>
        event PostResolveIncludeDelegate PostResolveInclude;

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
