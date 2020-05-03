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
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextModel : IContextModel
    {
        public ContextSite Site { get; }
        public ContextPage Page { get; }

        public ContextModel(ISite site, IPage page)
        {
            Site = new ContextSite(site);
            Page = new ContextPage(site, page);
        }
    }
}
