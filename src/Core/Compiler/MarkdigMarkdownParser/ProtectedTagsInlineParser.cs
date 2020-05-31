//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Markdig.Helpers;
using Markdig.Parsers;
using System.Text;

namespace Xarial.Docify.Core.Compiler.MarkdigMarkdownParser
{
    public class ProtectedTagsInlineParser : InlineParser
    {
        private readonly string m_StartTag;
        private readonly string m_EndTag;

        public ProtectedTagsInlineParser(string startTag, string endTag)
        {
            m_StartTag = startTag;
            m_EndTag = endTag;
            OpeningCharacters = new char[] { startTag[0] };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (!slice.Match(m_StartTag))
            {
                return false;
            }

            var rawContent = new StringBuilder();

            var current = slice.CurrentChar;

            while (!rawContent.ToString().EndsWith(m_EndTag))
            {
                if (slice.IsEmpty)
                {
                    return false;
                }

                rawContent.Append(current);
                current = slice.NextChar();
            }

            processor.Inline = new ProtectedTagsData(rawContent.ToString());

            return true;
        }
    }
}
