//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    public interface IPage : IFrame
    {
        List<IPage> SubPages { get; }
        List<IFile> Assets { get; }
        ILocation Location { get; }
    }

    public static class PageExtension
    {
        public static IEnumerable<IPage> GetAllSubPages(this IPage page)
        {
            if (page.SubPages != null)
            {
                foreach (var childPage in page.SubPages)
                {
                    yield return childPage;

                    foreach (var subChildPage in GetAllSubPages(childPage))
                    {
                        yield return subChildPage;
                    }
                }
            }
        }
    }
}
