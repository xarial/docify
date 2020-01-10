//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Base
{
    public static class PageExtensions
    {
        public static IEnumerable<Page> GetAllPages(this Page page)
        {
            yield return page;

            if (page.Children != null)
            {
                foreach (var childPage in page.Children)
                {
                    foreach (var subPage in GetAllPages(childPage)) 
                    {
                        yield return subPage;
                    }
                }
            }
        }
    }
}
