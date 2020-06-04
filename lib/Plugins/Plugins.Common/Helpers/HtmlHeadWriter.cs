//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Linq;
using System.Text;

namespace Xarial.Docify.Lib.Plugins.Common.Helpers
{
    public interface IHeadWriter
    {
        void AddStyleSheets(params string[] paths);
        void AddScripts(params string[] paths);
        void AddLines(params string[] lines);
    }

    public class HtmlHeadWriter : IHeadWriter
    {
        private const string CSS_LINK_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />\r\n";
        private const string SCRIPT_LINK_TEMPLATE = "<script src=\"{0}\"></script>";

        private StringBuilder m_Content;

        public HtmlHeadWriter(StringBuilder content)
        {
            m_Content = content;
        }

        public void AddScripts(params string[] paths)
            => AddLines(paths.Select(p => string.Format(SCRIPT_LINK_TEMPLATE, p)).ToArray());

        public void AddStyleSheets(params string[] paths)
            => AddLines(paths.Select(p => string.Format(CSS_LINK_TEMPLATE, p)).ToArray());

        public void AddLines(params string[] lines)
        {            
            var headInd = IndexOfHead();

            if (headInd != -1)
            {
                m_Content.Insert(headInd, string.Join(Environment.NewLine, lines));
            }
            else
            {
                throw new Exception("Failed to find </head> tag in the html document");
            }
        }

        private int IndexOfHead()
        {
            bool CompareChars(char x, char b) => Char.ToLower(x) == Char.ToLower(b);

            const string HEAD_NODE = "</head>";

            int searchValLength = HEAD_NODE.Length;
            
            for (int i = 0; i < m_Content.Length - searchValLength + 1; ++i)
            {
                if (CompareChars(m_Content[i], HEAD_NODE.First()))
                {
                    var curSearchCharIndex = 1;

                    while (curSearchCharIndex < searchValLength
                        && CompareChars(m_Content[i + curSearchCharIndex], HEAD_NODE[curSearchCharIndex]))
                    {
                        curSearchCharIndex++;
                    }
                        
                    if (curSearchCharIndex == searchValLength)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }
    }
}
