using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin
{
    public class Engine : IEngine
    {
        public IIncludesHandlerManager Includes { get; }
        public ICompilerManager Compiler { get; }
        public IComposerManager Composer { get; }
        public IPublisherManager Publisher { get; }

        public Engine(IIncludesHandlerManager includes, 
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
