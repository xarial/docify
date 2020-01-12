//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using ColorCode;
using ColorCode.Styling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Plugins
{
    [Plugin("code-syntax-highlighter")]
    public class CodeSyntaxHighlighter : IRenderCodeBlockPlugin, IPreCompilePlugin, IPrePublishTextAssetPlugin
    {
        private readonly HtmlClassFormatter m_Formatter;

        private const string CSS_FILE_NAME = "syntax-highlight.css";
        private readonly string[] CSS_FILE_PATH = new string[] { "assets", "styles" };

        public CodeSyntaxHighlighter() 
        {
            var styles = StyleDictionary.DefaultLight;

            for (int i = 0; i < styles.Count; i++)
            {
                styles[i].ReferenceName = "c" + i;
            }

            m_Formatter = new HtmlClassFormatter(styles);
        }

        public void PreCompile(Site site)
        {
            var css = m_Formatter.GetCSSString();
            site.Assets.Add(new TextAsset(css, new Location(CSS_FILE_NAME, CSS_FILE_PATH)));
        }

        public void PrePublishTextAsset(ref Location loc, ref string content, out bool cancel)
        {
            cancel = false;

            if (string.Equals(Path.GetExtension(loc.FileName), ".html", StringComparison.InvariantCultureIgnoreCase)) 
            {
                var headInd = content.IndexOf("</head>");

                if (headInd != -1) 
                {
                    content = content.Insert(headInd, $"<link rel=\"stylesheet\" type=\"text/css\" href=\"/{string.Join('/', CSS_FILE_PATH)}/{CSS_FILE_NAME}\" />\r\n");
                }
            }
        }

        public void RenderCodeBlock(string rawCode, string lang, ref StringBuilder result)
        {
            var codeLang = Languages.FindById(lang);

            var formattedCode = m_Formatter.GetHtmlString(rawCode, codeLang);
            
            result = new StringBuilder($"<pre><code>{formattedCode}</code></pre>");
        }
    }
}
