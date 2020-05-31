//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Threading.Tasks;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Base.Services
{
    public interface IDynamicContentTransformer
    {
        Task<string> Transform(string content, string key, IContextModel model);
    }
}
