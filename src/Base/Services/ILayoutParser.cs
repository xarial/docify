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
    /// <summary>
    /// Service parses and resolves layouts
    /// </summary>
    public interface ILayoutParser
    {
        /// <summary>
        /// Validates of the layout's content is valid
        /// </summary>
        /// <param name="content"></param>
        void ValidateLayout(string content);

        /// <summary>
        /// Inserts the content into the layout
        /// </summary>
        /// <param name="layout">Content's layout</param>
        /// <param name="content">Content to insert into the layout</param>
        /// <param name="site">Job's site</param>
        /// <param name="page">Owner page for the content</param>
        /// <param name="url">Url of the owner page</param>
        /// <returns>Resolved content</returns>
        Task<string> InsertContent(ITemplate layout, string content, ISite site, IPage page, string url);
    }
}
