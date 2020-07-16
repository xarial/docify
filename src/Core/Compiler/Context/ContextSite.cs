//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextSite : IContextSite
    {
        internal ISite BaseSite { get; }
                
        public IContextPage MainPage => new ContextPage(BaseSite, BaseSite.MainPage, "");
        public IContextConfiguration Configuration => new ContextConfiguration(BaseSite.Configuration);
        public string GetFullUrl(string url) => BaseSite.GetFullUrl(url);

        public ContextSite(ISite site)
        {
            BaseSite = site;
        }
    }
}
