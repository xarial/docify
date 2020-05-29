//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.ScriptStyleOptimizer
{
    public class ScriptStyleOptimizerPluginSettings
    {
        public bool MinifyCss { get; set; }
        public bool MinifyJs { get; set; }
        public string[] AssetsScopePaths { get; set; }
        public bool DeleteUnusedCss { get; set; }
        public bool DeleteUnusedJs { get; set; }
        public CasesInsensitiveDictionary<string[]> Bundles { get; set; }

        public ScriptStyleOptimizerPluginSettings() 
        {
            Bundles = new CasesInsensitiveDictionary<string[]>();
            AssetsScopePaths = new string[0];
        }
    }
}
