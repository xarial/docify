namespace Xarial.Docify.Base.Plugins
{
    public interface IEngine
    {
        IIncludesHandlerManager Includes { get; }
        ICompilerManager Compiler { get; }
        IComposerManager Composer { get; }
        IPublisherManager Publisher { get; }
    }
}
