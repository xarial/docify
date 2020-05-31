//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    public interface IStaticContentTransformer
    {
        Task<string> Transform(string content);
    }
}
