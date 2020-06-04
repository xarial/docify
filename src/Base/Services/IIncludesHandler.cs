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
    /// Service manages the resolution of includes in the static content
    /// </summary>
    public interface IIncludesHandler
    {
        /// <summary>
        /// Resolves all registered includes
        /// </summary>
        /// <param name="rawContent">Raw content to resolve includes in</param>
        /// <param name="site">Site of the job</param>
        /// <param name="page">Owner page for the content</param>
        /// <param name="url">Url of the owner page</param>
        /// <returns>Resolved content</returns>
        Task<string> ResolveAll(string rawContent, ISite site, IPage page, string url);
    }
}
