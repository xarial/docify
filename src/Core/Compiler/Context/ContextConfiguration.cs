//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
