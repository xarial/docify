//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Collections.Generic;
using System.Linq;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompilerConfig
    {
        private const string COMPILABLE_ASSETS_FILTER_PARAM = "compilable-assets";

        public string[] CompilableAssetsFilter { get; }

        public BaseCompilerConfig(IConfiguration conf)
        {
            var filter = conf.GetParameterOrDefault<IEnumerable<string>>(COMPILABLE_ASSETS_FILTER_PARAM);

            if (filter != null)
            {
                CompilableAssetsFilter = filter.ToArray();
            }
            else
            {
                CompilableAssetsFilter = new string[]
                {
                    "*.xml", "*.json"
                };
            }
        }
    }
}
