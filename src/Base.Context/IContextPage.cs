//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Context
{
    /// <summary>
    /// Representing page in the context of the include
    /// </summary>
    public interface IContextPage
    {
        /// <summary>
        /// Relative URL of the page
        /// </summary>
        string Url { get; }

        /// <summary>
        /// Full url of the page, including the host name
        /// </summary>
        string FullUrl { get; }

        /// <summary>
        /// Metadata of this page
        /// </summary>
        IContextMetadata Data { get; }

        /// <summary>
        /// Collection of sub pages
        /// </summary>
        IReadOnlyList<IContextPage> SubPages { get; }
        
        /// <summary>
        /// Collection of assets
        /// </summary>
        IReadOnlyList<IContextAsset> Assets { get; }
    }
}
