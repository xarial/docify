//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service transforms the static content, e.g. markdown
    /// </summary>
    public interface IStaticContentTransformer
    {
        /// <summary>
        /// Transform static content
        /// </summary>
        /// <param name="content">Raw content</param>
        /// <returns>Resolved content</returns>
        Task<string> Transform(string content);
    }
}
