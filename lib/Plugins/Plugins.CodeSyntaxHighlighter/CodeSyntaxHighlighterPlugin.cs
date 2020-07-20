//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using ColorCode;
using ColorCode.Styling;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.CodeSyntaxHighlighter.Properties;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;
using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.CodeSyntaxHighlighter
{
    public class CodeSyntaxHighlighterPlugin : IPlugin<CodeSyntaxHighlighterSettings>
    {
        private CodeSyntaxHighlighterSettings m_Settings;

        private CodeColorizerBase m_Formatter;

        private const string CSS_FILE_PATH = "/_assets/styles/syntax-highlight.css";
        private const string CLIPBOARDJS_FILE_PATH = @"/_assets/scripts/clipboard.min.js";
        private const string COPYCODE_FILE_PATH = @"/_assets/scripts/copy-code-clipboard.js";
        private const string COPYCODE_ICON_FILE_PATH = @"/_assets/images/copy-code.svg";

        private IDocifyApplication m_App;

        public void Init(IDocifyApplication app, CodeSyntaxHighlighterSettings setts)
        {
            m_App = app;
            m_Settings = setts;

            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Compiler.RenderCodeBlock += OnRenderCodeBlock;
            m_App.Compiler.WritePageContent += OnWritePageContent;
        }

        private Task OnPreCompile(ISite site)
        {
            if (!m_Settings.EmbedStyle)
            {
                var css = (Formatter as HtmlClassFormatter).GetCSSString();
                css = css.Substring("body{background-color:#FFFFFFFF;} ".Length);//temp solution - find a better way
                AssetsHelper.AddTextAsset(css, site.MainPage, CSS_FILE_PATH);
            }

            if (m_Settings.AddCopyCodeButton) 
            {
                AssetsHelper.AddAsset(Resources.copy_code, site.MainPage, COPYCODE_ICON_FILE_PATH);
                AssetsHelper.AddTextAsset(Resources.clipboard_min, site.MainPage, CLIPBOARDJS_FILE_PATH);
                AssetsHelper.AddTextAsset(Resources.copycodeclipboard, site.MainPage, COPYCODE_FILE_PATH);
            }

            return Task.CompletedTask;
        }

        private bool TryFileLanguageCodeById(string lang, out ILanguage codeLang)
        {
            codeLang = null;

            if (!string.IsNullOrEmpty(lang))
            {
                codeLang = Languages.FindById(lang);

                if (codeLang == null)
                {
                    switch (lang.ToLower())
                    {
                        case "vba":
                        case "vbs":
                            codeLang = Languages.VbDotNet;
                            break;

                        case "xslt":
                        case "wxs":
                            codeLang = Languages.Xml;
                            break;

                        default:
                            m_App.Logger.LogWarning($"Language '{lang}' is not supported");
                            break;
                    }
                }
            }

            return codeLang != null;
        }

        private void OnRenderCodeBlock(string rawCode, string lang, string args, StringBuilder html)
        {
            XElement cont;

            if (TryFileLanguageCodeById(lang, out ILanguage codeLang))
            {
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

                cont = node.Element("div");
                cont.RemoveAttributes();
            }
            else
            {
                cont = new XElement("div");
                cont.Add(new XElement("pre", rawCode));
            }

            cont.Add(new XAttribute("class", "code-snippet-container"));
            var pre = cont.Element("pre");
            pre.Add(new XAttribute("class", $"code-snippet {lang} {args}"));

            if (m_Settings.AddCopyCodeButton)
            {
                var copyCodeBtn = new XElement("button");
                copyCodeBtn.Add(new XAttribute("class", "snippet-btn copy-code-btn"));
                copyCodeBtn.Add(new XAttribute("title", "Copy Code To Clipboard"));
                var img = new XElement("img");
                img.SetAttributeValue("src", COPYCODE_ICON_FILE_PATH);
                img.SetAttributeValue("alt", "Copy code to clipboard");
                img.SetAttributeValue("width", "16px");
                img.SetAttributeValue("height", "16px");
                copyCodeBtn.Add(img);

                pre.AddBeforeSelf(copyCodeBtn);
            }

            html.Clear();
            html.Append(cont);
        }

        private Task OnWritePageContent(StringBuilder html, IMetadata data, string url)
        {
            if (html.Length > 0)
            {
                try
                {
                    var writer = new HtmlHeadWriter(html);

                    if (!m_Settings.EmbedStyle)
                    {
                        writer.AddStyleSheets(CSS_FILE_PATH);
                    }

                    if (m_Settings.AddCopyCodeButton) 
                    {
                        writer.AddScripts(CLIPBOARDJS_FILE_PATH, COPYCODE_FILE_PATH);
                    }
                }
                catch (Exception ex)
                {
                    throw new HeadAssetLinkFailedException(CSS_FILE_PATH, url, ex);
                }
            }

            return Task.FromResult(html);
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
