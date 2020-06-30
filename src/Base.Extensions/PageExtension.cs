//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base
{
    public static class PageExtension
    {
        public static IEnumerable<IPage> GetAllPages(this IPage page)
        {
            yield return page;

            foreach (var childPage in page.SubPages)
            {
                foreach (var subPage in GetAllPages(childPage))
                {
                    yield return subPage;
                }
            }
        }
    }
}
