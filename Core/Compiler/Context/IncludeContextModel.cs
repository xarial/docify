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
    public class IncludeContextModel : ContextModel
    {
        public ContextMetadata Data { get; }

        public IncludeContextModel(Site site, Page page, Metadata data)
            : base(site, page)
        {
            Data = new ContextMetadata(data);
        }
    }
}
