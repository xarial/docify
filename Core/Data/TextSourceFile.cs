//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class TextSourceFile : ITextSourceFile
    {
        public Location Location { get; }
        public string Content { get; }

        public TextSourceFile(Location path, string content)
        {
            Location = path;
            Content = content;
        }
    }
}
