//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Content
{
    public class TextAsset : Asset, ICompilable
    {
        public string RawContent { get; }
        public string Content { get; set; }
        public string Key => Location.ToId();

        public TextAsset(string rawContent, Location loc) : base(loc)
        {
            RawContent = rawContent;
        }
    }
}
