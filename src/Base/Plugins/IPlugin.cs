//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.ComponentModel;

namespace Xarial.Docify.Base.Plugins
{
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public interface IPluginBase
    {
    }

    /// <summary>
    /// Represents the plugin interface
    /// </summary>
    public interface IPlugin : IPluginBase
    {
        /// <summary>
        /// Called when plugin is initialized
        /// </summary>
        /// <param name="app">Pointer to applicatin</param>
        void Init(IDocifyApplication app);
    }

    /// <summary>
    /// Represents the plugin with settings
    /// </summary>
    /// <typeparam name="TSettings">Class representing plugins settings</typeparam>
    public interface IPlugin<TSettings> : IPluginBase
        where TSettings : new()
    {
        /// <inheritdoc cref="IPlugin.Init(IDocifyApplication)"/>
        /// <param name="setts">Settings for this plugin</param>
        void Init(IDocifyApplication app, TSettings setts);
    }
}
