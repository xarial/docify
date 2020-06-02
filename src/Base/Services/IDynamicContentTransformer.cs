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
    /// <summary>
    /// Sevice transforms pseudo-dynamic content, containing variables and code such as Razor pages
    /// </summary>
    public interface IDynamicContentTransformer
    {
        /// <summary>
        /// Transforms raw content to the resolved content
        /// </summary>
        /// <param name="content">Raw content to transform</param>
        /// <param name="key">Id of the content</param>
        /// <param name="model">Context model</param>
        /// <returns>Resolved content</returns>
        Task<string> Transform(string content, string key, IContextModel model);
    }
}
