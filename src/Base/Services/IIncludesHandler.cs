//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    public interface IIncludesHandler
    {
        Task<string> ResolveAll(string rawContent, ISite site, IPage page, string url);
    }
}
