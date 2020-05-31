//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Syntax.Inlines;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ProtectedTagsData : LeafInline
    {
        public string Content { get; }

        public ProtectedTagsData(string content)
        {
            Content = content;
        }
    }
}
