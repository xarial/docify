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

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextSite
    {
        internal ISite BaseSite { get; }

        public string BaseUrl 
        {
            get 
            {
                return BaseSite.BaseUrl;
            }
        }

        public ContextPage MainPage 
        {
            get 
            {
                return new ContextPage(BaseSite, BaseSite.MainPage);
            }
        }

        public ContextConfiguration Configuration 
        {
            get 
            {
                return new ContextConfiguration(BaseSite.Configuration);
            }
        }

        public ContextSite(ISite site) 
        {
            BaseSite = site;
        }
    }
}
