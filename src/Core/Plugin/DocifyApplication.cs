//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Plugin
{
    public class DocifyApplication : IDocifyApplication
    {
        public ILogger Logger { get; }
        public IIncludesHandlerManager Includes { get; }
        public ICompilerManager Compiler { get; }
        public IComposerManager Composer { get; }
        public IPublisherManager Publisher { get; }
        public ILoaderManager Loader { get; }

        public IEnumerable<IPluginBase> Plugins => m_PluginsMgr.Plugins;

        private readonly IPluginsManager m_PluginsMgr;

        public DocifyApplication(IIncludesHandlerManager includes,
            ICompilerManager compiler,
            IComposerManager composer,
            IPublisherManager publisher,
            ILoaderManager loader,
            ILogger logger,
            IPluginsManager pluginsMgr)
        {
            Includes = includes;
            Compiler = compiler;
            Composer = composer;
            Publisher = publisher;
            Loader = loader;
            Logger = logger;
            m_PluginsMgr = pluginsMgr;
        }
    }
}
