//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Plugins
{
    [Plugin("code-syntax-highlighter")]
    public class CodeSyntaxHighlighter : IRenderCodeBlockPlugin
    {
        public void RenderCodeBlock(string content, ref string result)
        {
        }
    }
}
