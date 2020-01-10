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
    public class IncludeContextModel : ContextModel
    {
        public IReadOnlyDictionary<string, dynamic> Data { get; }

        public IncludeContextModel(Site site, Page page, Dictionary<string, dynamic> data)
            : base(site, page)
        {
            Data = data;
        }
    }
}
