//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    public interface IDocifyApplication
    {
        ILogger Logger { get; }
        IIncludesHandlerManager Includes { get; }
        ICompilerManager Compiler { get; }
        IComposerManager Composer { get; }
        IPublisherManager Publisher { get; }
    }
}
