//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
