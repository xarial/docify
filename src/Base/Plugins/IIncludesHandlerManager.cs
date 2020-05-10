//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

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
