//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;

namespace Xarial.Docify.Core.Exceptions
{
    public class SiteMainPageMissingException : Exception
    {
        public SiteMainPageMissingException() : base("Site main page is not found. Add index.(md/html/cshtml) page to the root folder of the site")
        {
        }
    }
}
