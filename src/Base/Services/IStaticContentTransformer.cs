//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    public interface IStaticContentTransformer
    {
        Task<string> Transform(string content);
    }
}
