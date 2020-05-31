//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
