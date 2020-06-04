//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    /// <summary>
    /// Plugin interface of <see cref="IComposer"/>
    /// </summary>
    public interface IComposerManager
    {
        /// <summary>
        /// Instance of the composer
        /// </summary>
        IComposer Instance { get; }
    }
}
