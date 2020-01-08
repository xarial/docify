﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
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
