//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Context
{
    /// <summary>
    /// Represents the configuration in the context of the include
    /// </summary>
    public interface IContextConfiguration : IContextMetadata
    {
        /// <summary>
        /// Current environment name
        /// </summary>
        string Environment { get; }
    }
}
