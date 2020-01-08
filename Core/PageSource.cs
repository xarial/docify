//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class PageSource : IPageSource
    {
        public string Path { get; }

        public string Content { get; }

        public IReadOnlyDictionary<string, string> Data { get; }

        public PageSource(string path, string content, IReadOnlyDictionary<string, string> data) 
        {
            Path = path;
            Content = content;
            Data = data;
        }
    }
}
