//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Lib.Tools
{
    public static class MarkdownHelper
    {
        public static IContentTransformer MarkdownTransformer { get; set; }

        public static Task<string> ToHtml(string raw)
        {
            return MarkdownTransformer.Transform(raw, Guid.NewGuid().ToString(), null);
        }
    }
}
