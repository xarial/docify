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

    public interface IPlugin : IPluginBase
    {
        void Init(IDocifyApplication app);
    }

    public interface IPlugin<TSettings> : IPluginBase
        where TSettings : new()
    {
        void Init(IDocifyApplication app, TSettings setts);
    }
}
