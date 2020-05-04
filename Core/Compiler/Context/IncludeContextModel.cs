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
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class IncludeContextModel : ContextModel, IIncludeContextModel
    {
        public IContextMetadata Data { get; }

        internal IncludeContextModel(ISite site, IPage page, IMetadata data)
            : base(site, page)
        {
            Data = new ContextMetadata(data);
        }
    }
}
