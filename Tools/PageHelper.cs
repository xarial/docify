//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Lib.Tools
{
    public static class PageHelper
    {
        public const string TITLE_ATT = "title_attribute";
        private const string DEFAULT_TITLE_ATT = "title";

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

        public static string GetTitle(IContextPage page, IContextMetadata data)
        {
            var title = page.Data.GetOrDefault<string>(DEFAULT_TITLE_ATT);

            if (string.IsNullOrEmpty(title))
            {
                title = page.Name;
            }

            return title;
        }

        /// <summary>
        /// Returns the caption (short title) of the page
        /// </summary>
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
    }
}
