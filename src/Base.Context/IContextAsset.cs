//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Context
{
    /// <summary>
    /// Representing an asset in the context of an include
    /// </summary>
    public interface IContextAsset
    {
        /// <summary>
        /// Content of the asset file
        /// </summary>
        byte[] Content { get; }

        /// <summary>
        /// Name of the file asset
        /// </summary>
        string Name { get; }
    }
}
