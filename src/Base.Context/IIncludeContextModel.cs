//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Context
{
    /// <summary>
    /// Context model passed to include
    /// </summary>
    public interface IContextModel
    {
        /// <summary>
        /// Current site
        /// </summary>
        IContextSite Site { get; }
        
        /// <summary>
        /// Current page (if include in the page) or the owner page of an asset, if include is in the asset
        /// </summary>
        IContextPage Page { get; }

        /// <summary>
        /// Current metadata of this include (this metadata is a combination of include metadata and all overrides)
        /// </summary>
        IContextMetadata Data { get; }
    }
}
