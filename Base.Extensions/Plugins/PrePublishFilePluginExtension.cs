using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Plugins
{
    public interface IHeadWriter
    {
        void AddStyleSheet(string path);
        void AddScript(string path);
        void AddLine(string line);
    }

    internal class HtmlFile : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }

        internal HtmlFile(string content, ILocation loc)
        {
            Content = ContentExtension.ToByteArray(content);
            Location = loc;
        }
    }

    internal class HtmlHeadWriter : IHeadWriter
    {
        private const string CSS_LINK_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" href=\"/{0}\" />\r\n";
        private const string SCRIPT_LINK_TEMPLATE = "<script src=\"{0}\"></script>";

        internal string Content 
        {
            get 
            {
                if (!HasChanged) 
                {
                    throw new Exception("Content hasn't changed");
                }

                if (!m_IsContentLoaded) 
                {
                    m_Content = m_File.AsTextContent();
                    m_IsContentLoaded = true;
                }

                return m_Content;
            }
            private set
            {
                m_Content = value;
            }
        }

        private bool m_IsContentLoaded;
        private string m_Content;
        private readonly IFile m_File;

        internal bool HasChanged { get; private set; }

        internal HtmlHeadWriter(IFile file) 
        {
            m_File = file;

            m_IsContentLoaded = false;
            HasChanged = false;
        }

        public void AddScript(string path) => AddLine(string.Format(SCRIPT_LINK_TEMPLATE, path));
        public void AddStyleSheet(string path) => AddLine(string.Format(CSS_LINK_TEMPLATE, path));

        public void AddLine(string line)
        {
            HasChanged = true;

            var headInd = Content.IndexOf("</head>");

            if (headInd != -1)
            {
                Content = Content.Insert(headInd, line);
            }
            else
            {
                throw new Exception("Failed to find </head> tag in the html document");
            }
        }
    }

    public static class PrePublishFilePluginExtension
    {
        private static bool IsHtmlPage(IFile file) => string.Equals(Path.GetExtension(file.Location.FileName), ".html",
                StringComparison.InvariantCultureIgnoreCase);

        public static IFile WriteToPageHead(this IPrePublishFilePlugin plugin, 
            IFile file, Action<IHeadWriter> writer)
        {
            if (IsHtmlPage(file)) 
            {
                var htmlWriter = new HtmlHeadWriter(file);
                writer.Invoke(htmlWriter);
                
                if (htmlWriter.HasChanged) 
                {
                    return new HtmlFile(htmlWriter.Content, file.Location);
                }
            }

            return file;
        }
    }
}
