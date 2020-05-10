using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Plugin
{
    public class ComposerManager : IComposerManager
    {
        public IComposer Instance { get; }
        private readonly ComposerExtension m_Ext;

        public ComposerManager(IComposer inst, ComposerExtension ext)
        {
            Instance = inst;
            m_Ext = ext;
        }
    }
}
