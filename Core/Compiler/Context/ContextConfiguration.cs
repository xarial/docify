using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextConfiguration : ContextMetadata, IContextConfiguration
    {
        public Environment_e Environment { get; }

        public ContextConfiguration(IConfiguration conf) : base(conf) 
        {
            Environment = conf.Environment;
        }
    }
}
