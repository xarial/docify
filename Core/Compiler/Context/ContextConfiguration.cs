//*********************************************************************
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
using Xarial.Docify.Core.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextConfiguration : ContextMetadata
    {
        public Environment_e Environment { get; set; }

        public ContextConfiguration(IConfiguration config)
            : base(config) 
        {
            Environment = config.Environment;
        }
    }
}
