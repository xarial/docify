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

namespace Xarial.Docify.Core.Compiler
{
    public class ContextModel : IContextModel
    {
        public Site Site { get; }
        public Page Page { get; }

        public ContextModel(Site site, Page page)
        {
            Site = site;
            Page = page;
        }
    }
}
