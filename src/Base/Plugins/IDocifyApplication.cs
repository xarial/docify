//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    /// <summary>
    /// Top level API object with an access to all API services
    /// </summary>
    public interface IDocifyApplication
    {
        IEnumerable<IPluginBase> Plugins { get; }

        /// <summary>
        /// Instance of the logger to log user messages
        /// </summary>
        ILogger Logger { get; }

        /// <summary>
        /// Includes service API
        /// </summary>
        IIncludesHandlerManager Includes { get; }
        
        /// <summary>
        /// Compiler service API
        /// </summary>
        ICompilerManager Compiler { get; }
        
        /// <summary>
        /// Composer service API
        /// </summary>
        IComposerManager Composer { get; }
        
        /// <summary>
        /// Publisher service API
        /// </summary>
        IPublisherManager Publisher { get; }

        /// <summary>
        /// Loader service API
        /// </summary>
        ILoaderManager Loader { get; }
    }
}
