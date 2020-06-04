//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Lib.Tools
{
    /// <summary>
    /// Provides helper methods for the page
    /// </summary>
    public static class PageHelper
    {
        public const string TITLE_ATT = "title-attribute";
        private const string DEFAULT_TITLE_ATT = "title";

        /// <summary>
        /// Gets all pages from all level
        /// </summary>
        /// <param name="page">This page</param>
        /// <returns>All pages</returns>
        public static IEnumerable<IContextPage> GetAllPages(IContextPage page)
        {
            if (page.SubPages != null)
            {
                foreach (var childPage in page.SubPages)
                {
                    yield return childPage;

                    foreach (var subChildPage in GetAllPages(childPage))
                    {
                        yield return subChildPage;
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the title from this page
        /// </summary>
        /// <param name="page">This page</param>
        /// <param name="data">Page metadata</param>
        /// <returns>Title</returns>
        public static string GetTitle(IContextPage page, IContextMetadata data)
        {
            var title = page.Data.GetOrDefault<string>(DEFAULT_TITLE_ATT);

            if (string.IsNullOrEmpty(title))
            {
                title = page.Url.Split('/', StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            }

            return title;
        }

        /// <summary>
        /// Returns the caption (short title) of the page
        /// </summary>
        /// <param name="page">This page</param>
        /// <param name="data">Page metadata</param>
        /// <returns>Caption</returns>
        public static string GetCaption(IContextPage page, IContextMetadata data)
        {
            var caption = "";

            if (data != null)
            {
                string titleAtt;

                if (data.TryGet(TITLE_ATT, out titleAtt) && !string.IsNullOrEmpty(titleAtt))
                {
                    caption = page.Data.GetOrDefault<string>(titleAtt);
                }
            }

            if (string.IsNullOrEmpty(caption))
            {
                caption = GetTitle(page, data);
            }

            return caption;
        }

        /// <summary>
        /// Finds the language of this page
        /// </summary>
        /// <param name="site">Site</param>
        /// <param name="page">Page to get language for</param>
        /// <returns>Language</returns>
        public static string GetLanguage(IContextSite site, IContextPage page)
        {
            const string LANG_VAR = "lang";

            var lang = page.Data.GetOrDefault<string>(LANG_VAR);

            if (string.IsNullOrEmpty(lang))
            {
                lang = site.Configuration.GetOrDefault<string>(LANG_VAR);
            }

            return lang;
        }
    }
}
