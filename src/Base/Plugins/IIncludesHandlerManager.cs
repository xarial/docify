using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    public delegate Task<string> ResolveCustomIncludeDelegate(IMetadata data, IPage page);

    public interface IIncludesHandlerManager
    {
        IIncludesHandler Instance { get; }
        void RegisterCustomIncludeHandler(string includeName, ResolveCustomIncludeDelegate handler);
    }
}
