//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Template : Sheet, ITemplate
    {
        public Template(string name, string rawContent, string id,
            IMetadata data = null, ITemplate baseTemplate = null)
            : base(rawContent, name, data, baseTemplate, id)
        {
        }
    }
}
