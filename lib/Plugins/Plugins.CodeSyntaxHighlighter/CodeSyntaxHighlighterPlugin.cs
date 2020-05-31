//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
using Xarial.Docify.Lib.Plugins.Common.Exceptions;
using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.CodeSyntaxHighlighter
{
    [Plugin("code-syntax-highlighter")]
    public class CodeSyntaxHighlighterPlugin : IPlugin<CodeSyntaxHighlighterSettings>
    {
        private CodeSyntaxHighlighterSettings m_Settings;

        private CodeColorizerBase m_Formatter;

        private const string CSS_FILE_PATH = "/assets/styles/syntax-highlight.css";

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

            html.Clear();
            html.Append(cont);
        }

        private Task<string> OnWritePageContent(string content, IMetadata data, string url)
        {
            if (!m_Settings.EmbedStyle)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        var writer = new HtmlHeadWriter(content);
                        writer.AddStyleSheets(CSS_FILE_PATH);
                        content = writer.Content;
                    }
                    catch (Exception ex)
                    {
                        throw new HeadAssetLinkFailedException(CSS_FILE_PATH, url, ex);
                    }
                }
                else
                {
                    return Task.FromResult(content);
                }
            }

            return Task.FromResult(content);
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
