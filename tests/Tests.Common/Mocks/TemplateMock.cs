﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Data;

namespace Tests.Common.Mocks
{
    public class TemplateMock : Template
    {
        public TemplateMock(string name, string rawContent, IMetadata md = null, Template baseTemplate = null)
            : base(name, rawContent, Guid.NewGuid().ToString(), md, baseTemplate)
        {
        }
    }
}
