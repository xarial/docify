//*********************************************************************
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
    public class SourceFile : ISourceFile
    {
        public Location Path { get; }
        public string Content { get; }

        public SourceFile(Location path, string content) 
        {
            Path = path;
            Content = content;
        }
    }
}
