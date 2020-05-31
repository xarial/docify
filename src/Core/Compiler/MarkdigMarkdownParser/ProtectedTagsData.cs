//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
