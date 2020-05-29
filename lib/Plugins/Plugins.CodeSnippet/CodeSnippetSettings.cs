//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.CodeSnippet
{
    public class CodeSnippetSettings
    {
        public string SnippetsFolder { get; set; } = "";
        public bool ExcludeSnippets { get; set; } = true;
        public CasesInsensitiveDictionary<string> AutoTabs { get; set; }
    }
}
