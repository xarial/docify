//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Linq;

namespace Xarial.Docify.Base.Plugins
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

        //TODO: implement this via StringBuilder

        public string Content { get; private set; }

        public HtmlHeadWriter(string content)
        {
            Content = content;
        }

        public void AddScripts(params string[] paths)
            => AddLines(paths.Select(p => string.Format(SCRIPT_LINK_TEMPLATE, p)).ToArray());

        public void AddStyleSheets(params string[] paths)
            => AddLines(paths.Select(p => string.Format(CSS_LINK_TEMPLATE, p)).ToArray());

        public void AddLines(params string[] lines)
        {
            var headInd = Content.IndexOf("</head>");

            if (headInd != -1)
            {
                Content = Content.Insert(headInd, string.Join(Environment.NewLine, lines));
            }
            else
            {
                throw new Exception("Failed to find </head> tag in the html document");
            }
        }
    }
}
