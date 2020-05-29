//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Lib.Plugins.CodeSnippet
{
    public class CodeSnippetData
    {
        public string FileName { get; set; }
        public Dictionary<string, string> Tabs { get; set; }
        public string[] Regions { get; set; }
        public string[] ExclRegions { get; set; }
        public bool LeftAlign { get; set; } = true;
        public string Lang { get; set; }
    }
}
