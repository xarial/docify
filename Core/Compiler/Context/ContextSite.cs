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
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextSite : IContextSite
    {
        internal ISite BaseSite { get; }

        public string BaseUrl => BaseSite.BaseUrl;
        public IContextPage MainPage => new ContextPage(BaseSite, BaseSite.MainPage);
        public IContextConfiguration Configuration => new ContextConfiguration(BaseSite.Configuration);

        public ContextSite(ISite site) 
        {
            BaseSite = site;
        }
    }
}
