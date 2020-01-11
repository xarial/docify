﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextConfiguration : ContextMetadata, IConfiguration
    {
        public Environment_e Environment { get; set; }

        public ContextConfiguration(Configuration config)
            : base(config) 
        {
            Environment = config.Environment;
        }
    }
}