//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.CodeSnippet.Helpers;
using Xarial.Docify.Lib.Plugins.CodeSnippet.Properties;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;
using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.CodeSnippet
{
    [Plugin("code-snippet")]
    public class CodeSnippetPlugin : IPlugin<CodeSnippetSettings>
    {
        private CodeSnippetSettings m_Settings;

        private const string CSS_FILE_PATH = "/assets/styles/code-snippet.css";
        private const string JS_FILE_PATH = "/assets/scripts/code-snippet.js";
        private const char SNIPPETS_FOLDER_PATH = '~';

        private IAssetsFolder m_SnippetsFolder;
        private List<string> m_SnippetFileIds;

        private ISite m_Site;

        private IDocifyApplication m_App;

        public void Init(IDocifyApplication app, CodeSnippetSettings setts)
        {
            m_App = app;
            m_Settings = setts;

            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Includes.RegisterCustomIncludeHandler("code-snippet", InsertCodeSnippet);
            m_App.Publisher.PrePublishFile += OnPrePublishFile;
            m_App.Compiler.WritePageContent += OnWritePageContent;
        }

        private Task OnPreCompile(ISite site)
        {
            m_Site = site;

            AssetsHelper.AddTextAsset(Resources.code_snippet_css, site.MainPage, CSS_FILE_PATH);
            AssetsHelper.AddTextAsset(Resources.code_snippet_js, site.MainPage, JS_FILE_PATH);

            m_SnippetFileIds = new List<string>();

            if (!string.IsNullOrEmpty(m_Settings.SnippetsFolder))
            {
                m_SnippetsFolder = site.MainPage;

                var parts = m_Settings.SnippetsFolder.Split(AssetsHelper.PathSeparators,
                    StringSplitOptions.RemoveEmptyEntries);

                foreach (var part in parts)
                {
                    var nextFolder = m_SnippetsFolder.Folders
                        .FirstOrDefault(f => string.Equals(f.Name, part, StringComparison.CurrentCultureIgnoreCase));

                    if (nextFolder == null)
                    {
                        if (m_SnippetsFolder is IPage)
                        {
                            nextFolder = (m_SnippetsFolder as IPage).SubPages
                                .FirstOrDefault(p => string.Equals(p.Name, part, StringComparison.CurrentCultureIgnoreCase));
                        }
                    }

                    if (nextFolder == null)
                    {
                        throw new Exception($"Failed to find the folder for snippets: '{m_Settings.SnippetsFolder}'");
                    }

                    m_SnippetsFolder = nextFolder;
                }

                foreach (var snipAsset in AssetsHelper.GetAllAssets(m_SnippetsFolder))
                {
                    m_SnippetFileIds.Add(snipAsset.Id);
                }
            }

            return Task.CompletedTask;
        }

        private async Task<string> InsertCodeSnippet(IMetadata data, IPage page)
        {
            var snipData = data.ToObject<CodeSnippetData>();

            if (!string.IsNullOrEmpty(snipData.FileName)
                && snipData.Tabs?.Any() == true)
            {
                throw new Exception("Specify either file name or tabs");
            }

            if (snipData.FileName?.EndsWith(".*") == true)
            {
                if (m_Settings.AutoTabs?.Any() != true)
                {
                    throw new Exception($"{nameof(m_Settings.AutoTabs)} setting must be set to use automatic code snippet tabs");
                }

                var fileName = Path.GetFileName(snipData.FileName);
                var dir = snipData.FileName.Substring(0, snipData.FileName.Length - fileName.Length);
                var snipsFolder = AssetsHelper.FindAssetsFolder(
                    m_Site, page,
                    AssetsHelper.LocationFromPath(dir));

                snipData.Tabs = new Dictionary<string, string>();

                foreach (var asset in snipsFolder.Assets
                    .Where(a => string.Equals(Path.GetFileNameWithoutExtension(a.FileName),
                    Path.GetFileNameWithoutExtension(fileName), StringComparison.CurrentCultureIgnoreCase)))
                {
                    string ext = Path.GetExtension(asset.FileName).TrimStart('.');

                    if (m_Settings.AutoTabs.TryGetValue(ext, out string tabName))
                    {
                        snipData.Tabs.Add(tabName, dir + asset.FileName);
                    }
                }
            }

            if (snipData.Tabs?.Any() == true)
            {
                var tabsHtml = new StringBuilder();
                var tabsCode = new StringBuilder();

                bool isFirst = true;

                var tabId = ConvertToId(snipData.Tabs.First().Value.Substring(0, snipData.Tabs.First().Value.LastIndexOf(".")));

                foreach (var tab in snipData.Tabs)
                {
                    var tabName = tab.Key;
                    var tabFile = tab.Value;

                    var snipId = ConvertToId(tabFile);

                    tabsHtml.AppendLine($"<button class=\"tablinks{(isFirst ? " active" : "")}\" onclick=\"openTab(event, '{tabId}', '{snipId}')\">{HttpUtility.HtmlEncode(tabName)}</button>");

                    tabsCode.AppendLine($"<div id=\"{snipId}\" class=\"tabcontent\" style=\"display: {(isFirst ? "block" : "none")}\">");
                    await WriteCodeSnippet(tabsCode, page, tabFile, snipData.Lang,
                        snipData.ExclRegions, snipData.LeftAlign, snipData.Regions);
                    tabsCode.AppendLine("</div>");

                    isFirst = false;
                }

                return string.Format(Resources.code_snippet_tab_container, tabId, tabsHtml.ToString(), tabsCode.ToString());
            }
            else
            {
                var html = new StringBuilder();

                await WriteCodeSnippet(html, page, snipData.FileName, snipData.Lang,
                    snipData.ExclRegions, snipData.LeftAlign, snipData.Regions);

                return html.ToString();
            }
        }

        private string ConvertToId(string val) => val
            .Replace(".", "-")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("::", "-")
            .Replace(" ", "-");

        private async Task WriteCodeSnippet(StringBuilder html, IPage page,
            string fileName, string lang, string[] exclRegs, bool leftAlign, string[] regs)
        {
            IAsset snipAsset;

            try
            {
                IAssetsFolder searchFolder = null;

                if (fileName.StartsWith(SNIPPETS_FOLDER_PATH))
                {
                    if (m_SnippetsFolder == null)
                    {
                        throw new Exception("Snippets folder is not set");
                    }

                    fileName = fileName.TrimStart(SNIPPETS_FOLDER_PATH);
                    searchFolder = m_SnippetsFolder;
                }
                else
                {
                    searchFolder = page;
                }

                snipAsset = AssetsHelper.FindAsset(m_Site, searchFolder, fileName);
            }
            catch (Exception ex)
            {
                throw new NullReferenceException($"Failed to find code snippet: '{fileName}'", ex);
            }

            if (snipAsset != null)
            {
                if (!m_SnippetFileIds.Contains(snipAsset.Id))
                {
                    m_SnippetFileIds.Add(snipAsset.Id);
                }

                var rawCode = snipAsset.AsTextContent();

                if (string.IsNullOrEmpty(lang))
                {
                    lang = Path.GetExtension(snipAsset.FileName).TrimStart('.').ToLower();
                }

                var snips = CodeSnippetHelper.Select(rawCode, lang, new CodeSelectorOptions()
                {
                    ExcludeRegions = exclRegs,
                    LeftAlign = leftAlign,
                    Regions = regs
                });

                foreach (var snip in snips)
                {
                    var snipClass = "";

                    switch (snip.Location)
                    {
                        case SnippetLocation_e.Full:
                            snipClass = "";
                            break;

                        case SnippetLocation_e.Start:
                            snipClass = "jagged-bottom";
                            break;

                        case SnippetLocation_e.End:
                            snipClass = "jagged-top";
                            break;

                        case SnippetLocation_e.Middle:
                            snipClass = "jagged";
                            break;
                    }

                    var code = $"~~~{lang} {snipClass}\r\n{snip.Code}\r\n~~~";
                    html.AppendLine(await m_App.Compiler.StaticContentTransformer.Transform(code));
                }
            }
            else
            {
                throw new InvalidCastException($"Failed to find an asset at '{fileName}'");
            }
        }

        private Task<PrePublishResult> OnPrePublishFile(ILocation outLoc, IFile file)
        {
            var res = new PrePublishResult()
            {
                File = file,
                SkipFile = m_Settings.ExcludeSnippets
                    && m_SnippetFileIds.Contains(file.Id)
            };

            return Task.FromResult(res);
        }

        private Task<string> OnWritePageContent(string content, IMetadata data, string url)
        {
            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var writer = new HtmlHeadWriter(content);
                    writer.AddStyleSheets(CSS_FILE_PATH);
                    writer.AddScripts(JS_FILE_PATH);
                    return Task.FromResult(writer.Content);
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
    }
}
