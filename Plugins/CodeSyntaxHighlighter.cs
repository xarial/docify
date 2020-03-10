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

namespace Xarial.Docify.Lib.Plugins
{
    public class CodeSyntaxHighlighterSettings
    {
        public bool EmbedStyle { get; set; } = true;
    }

    [Plugin("code-syntax-highlighter")]
    public class CodeSyntaxHighlighter : IRenderCodeBlockPlugin, IPreCompilePlugin, IPrePublishTextAssetPlugin, IPlugin<CodeSyntaxHighlighterSettings>
    {
        public CodeSyntaxHighlighterSettings Settings { get; set; }

        private CodeColorizerBase m_Formatter;

        private const string CSS_FILE_NAME = "syntax-highlight.css";
        private readonly string[] CSS_FILE_PATH = new string[] { "assets", "styles" };
        
        public void PreCompile(Site site)
        {
            if (!Settings.EmbedStyle)
            {
                var css = (Formatter as HtmlClassFormatter).GetCSSString();
                css = css.Substring("body{background-color:#FFFFFFFF;} ".Length);//temp solution - find a better way
                site.Assets.Add(new TextAsset(css, new Location(CSS_FILE_NAME, CSS_FILE_PATH)));
            }
        }

        public void PrePublishTextAsset(ref Location loc, ref string content, out bool cancel)
        {
            cancel = false;

            if (!Settings.EmbedStyle)
            {
                if (string.Equals(Path.GetExtension(loc.FileName), ".html", StringComparison.InvariantCultureIgnoreCase))
                {
                    var headInd = content.IndexOf("</head>");

                    if (headInd != -1)
                    {
                        content = content.Insert(headInd, $"<link rel=\"stylesheet\" type=\"text/css\" href=\"/{string.Join('/', CSS_FILE_PATH)}/{CSS_FILE_NAME}\" />\r\n");
                    }
                }
            }
        }

        public void RenderCodeBlock(string rawCode, string lang, ref StringBuilder result)
        {
            var codeLang = Languages.FindById(lang);

            string formattedCode;
            if (Formatter is HtmlFormatter)
            {
                formattedCode = (Formatter as HtmlFormatter).GetHtmlString(rawCode, codeLang);
            }
            else if (Formatter is HtmlClassFormatter)
            {
                formattedCode = (Formatter as HtmlClassFormatter).GetHtmlString(rawCode, codeLang);
            }
            else
            {
                throw new NotSupportedException("Incorrect formatted");
            }

            result = new StringBuilder($"<pre class=\"code-snippet\"><code>{formattedCode}</code></pre>");
        }

        private CodeColorizerBase Formatter
        {
            get
            {
                if (m_Formatter == null)
                {
                    var styles = StyleDictionary.DefaultLight;

                    for (int i = 0; i < styles.Count; i++)
                    {
                        styles[i].ReferenceName = "c" + i;
                    }

                    if (Settings.EmbedStyle)
                    {
                        m_Formatter = new HtmlFormatter(styles);
                    }
                    else
                    {
                        m_Formatter = new HtmlClassFormatter(styles);
                    }
                }

                return m_Formatter;
            }
        }
    }
}
