//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin
{
    public class DocifyApplication : IDocifyApplication
    {
        public IIncludesHandlerManager Includes { get; }
        public ICompilerManager Compiler { get; }
        public IComposerManager Composer { get; }
        public IPublisherManager Publisher { get; }

        public DocifyApplication(IIncludesHandlerManager includes, 
            ICompilerManager compiler, 
            IComposerManager composer, 
            IPublisherManager publisher)
        {
            Includes = includes;
            Compiler = compiler;
            Composer = composer;
            Publisher = publisher;
        }
    }
}
