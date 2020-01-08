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
    public class ElementSource : IElementSource
    {
        public string Path { get; }
        public string Content { get; }
        public ElementType_e Type { get; }

        public ElementSource(string path, string content, ElementType_e type) 
        {
            Path = path;
            Content = content;
            Type = type;
        }
    }
}
