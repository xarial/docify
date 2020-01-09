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

namespace Xarial.Docify.Core.Compiler
{
    public class IncludesContextModel : ContextModel
    {
        public Dictionary<string, dynamic> Parameters { get; }

        public IncludesContextModel(Site site, Page page, Dictionary<string, dynamic> parameters)
            : base(site, page)
        {
            Parameters = parameters;
        }
    }
}
