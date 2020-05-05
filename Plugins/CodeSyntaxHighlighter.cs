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
using System.Xml.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Lib.Plugins
{
    public class CodeSyntaxHighlighterSettings
    {
        public bool EmbedStyle { get; set; } = true;
    }

    [Plugin("code-syntax-highlighter")]
    public class CodeSyntaxHighlighter : IRenderCodeBlockPlugin, IPreCompilePlugin, IPrePublishFilePlugin, IPlugin<CodeSyntaxHighlighterSettings>
    {
        private CodeSyntaxHighlighterSettings m_Settings;

        private CodeColorizerBase m_Formatter;

        private const string CSS_FILE_NAME = "syntax-highlight.css";
        private readonly string[] CSS_FILE_PATH = new string[] { "assets", "styles" };

        public void Init(CodeSyntaxHighlighterSettings setts)
        {
            m_Settings = setts;
        }

        public void PreCompile(ISite site)
        {
            if (!m_Settings.EmbedStyle)
            {
                var css = (Formatter as HtmlClassFormatter).GetCSSString();
                css = css.Substring("body{background-color:#FFFFFFFF;} ".Length);//temp solution - find a better way
                site.MainPage.Assets.Add(new PluginDataFile(css, new PluginDataFileLocation(CSS_FILE_NAME, CSS_FILE_PATH)));
            }
        }

        public void PrePublishFile(ILocation outLoc, ref IFile file, out bool skip)
        {
            skip = false;

            if (!m_Settings.EmbedStyle)
            {
                if (string.Equals(Path.GetExtension(file.Location.FileName), ".html", StringComparison.InvariantCultureIgnoreCase))
                {
                    var pageContent = file.AsTextContent();
                    Helper.InjectDataIntoHtmlHead(ref pageContent,
                        string.Format(Helper.CSS_LINK_TEMPLATE, string.Join('/', CSS_FILE_PATH) + "/" + CSS_FILE_NAME));
                    file = new PluginDataFile(pageContent, file.Location);
                }
            }
        }

        private ILanguage FileLanguageCodeById(string lang) 
        {
            var codeLang = Languages.FindById(lang);

            if (codeLang == null) 
            {
                switch (lang.ToLower()) 
                {
                    case "vba":
                        codeLang = Languages.VbDotNet;
                        break;

                    default:
                        throw new NotSupportedException($"Language '{lang}' is not supported");
                }
            }

            return codeLang;
        }

        public void RenderCodeBlock(string rawCode, string lang, string args, StringBuilder html)
        {
            var codeLang = FileLanguageCodeById(lang);

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

            var node = XDocument.Parse(formattedCode, LoadOptions.PreserveWhitespace);

            var pre = node.Element("div").Element("pre");

            pre.Add(new XAttribute("class", $"code-snippet {lang} {args}"));

            html.Clear();
            html.Append(pre);
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

                    if (m_Settings.EmbedStyle)
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
