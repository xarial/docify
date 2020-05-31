//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
