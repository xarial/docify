//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextConfiguration : ContextMetadata, IContextConfiguration
    {
        public string Environment { get; }

        public ContextConfiguration(IConfiguration conf) : base(conf)
        {
            Environment = conf.Environment;
        }
    }
}
