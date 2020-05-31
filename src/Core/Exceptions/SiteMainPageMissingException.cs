//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
