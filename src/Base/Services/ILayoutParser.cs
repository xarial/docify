//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
