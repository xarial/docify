//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface ILayoutParser
    {
        void ValidateLayout(string content);
        Task<string> InsertContent(ITemplate layout, string content, ISite site, IPage page, string url);
    }
}
