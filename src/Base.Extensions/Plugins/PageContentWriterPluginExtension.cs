//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Plugins
{
    public interface IHeadWriter
    {
        void AddStyleSheets(params string[] paths);
        void AddScripts(params string[] paths);
        void AddLines(params string[] lines);
    }

    internal class HtmlHeadWriter : IHeadWriter
    {
        private const string CSS_LINK_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/{0}\" />\r\n";
        private const string SCRIPT_LINK_TEMPLATE = "<script src=\"{0}\"></script>";

        //TODO: implement this via StringBuilder

        internal string Content { get; private set; }

        internal HtmlHeadWriter(string content)
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

    public static class PageContentWriterPluginExtension
    {
        //TODO: fix
        //public static string WriteToPageHead(this IPageContentWriterPlugin plugin, 
        //    string content, Action<IHeadWriter> headWriter)
        //{
        //    var htmlWriter = new HtmlHeadWriter(content);
        //    headWriter.Invoke(htmlWriter);
        //    return htmlWriter.Content;
        //}
    }
}
