//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
using Xarial.Docify.Base.Exceptions;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.CodeSnippet.Helpers;
using Xarial.Docify.Lib.Plugins.CodeSnippet.Properties;
using Xarial.Docify.Lib.Plugins.Common.Data;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;
using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.CodeSnippet
{
    public interface ICodeSnippetPlugin 
    {
    }

    public class CodeSnippetPlugin : IPlugin<CodeSnippetSettings>, ICodeSnippetPlugin
    {
        private CodeSnippetSettings m_Settings;

        private const string CSS_FILE_PATH = "/_assets/styles/code-snippet.css";
        private const string JS_FILE_PATH = "/_assets/scripts/code-snippet.js";
        private const char SNIPPETS_FOLDER_PATH = '~';

        private IAssetsFolder m_SnippetsFolder;
        private List<string> m_SnippetFileIds;

        private Dictionary<IPage, List<string>> m_UsedTabIds;
        private Dictionary<IPage, List<string>> m_UsedSnippetIds;

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
            m_UsedTabIds = new Dictionary<IPage, List<string>>();
            m_UsedSnippetIds = new Dictionary<IPage, List<string>>();

            if (!string.IsNullOrEmpty(m_Settings.SnippetsFolder))
            {
                try
                {                     
                    m_SnippetsFolder = site.MainPage.FindFolder(PluginLocation.FromPath(m_Settings.SnippetsFolder));
                }
                catch (AssetNotFoundException) 
                {
                    throw new PluginUserMessageException($"Failed to find the folder for snippets: '{m_Settings.SnippetsFolder}'");
                }

                foreach (var snipAsset in m_SnippetsFolder.GetAllAssets())
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
                throw new PluginUserMessageException("Specify either file name or tabs");
            }

            if (snipData.FileName?.EndsWith(".*") == true)
            {
                if (m_Settings.AutoTabs?.Any() != true)
                {
                    throw new PluginUserMessageException($"{nameof(m_Settings.AutoTabs)} setting must be set to use automatic code snippet tabs");
                }

                var fileName = snipData.FileName;
                var snipsFolder = FindSnippetFolder(m_Site, page, ref fileName);
                
                snipData.Tabs = new Dictionary<string, string>();

                foreach (var asset in snipsFolder.Assets
                    .Where(a => string.Equals(Path.GetFileNameWithoutExtension(a.FileName),
                    Path.GetFileNameWithoutExtension(fileName), StringComparison.CurrentCultureIgnoreCase)))
                {
                    string ext = Path.GetExtension(asset.FileName).TrimStart('.');

                    if (m_Settings.AutoTabs.TryGetValue(ext, out string tabName))
                    {
                        snipData.Tabs.Add(tabName, Path.ChangeExtension(snipData.FileName, Path.GetExtension(asset.FileName)));
                    }
                }
            }

            if (snipData.Tabs?.Any() == true)
            {
                var tabsHtml = new StringBuilder();
                var tabsCode = new StringBuilder();

                bool isFirst = true;

                var tabId = ConvertToId(snipData.Tabs.First().Value
                    .Substring(0, snipData.Tabs.First().Value.LastIndexOf(".")));

                tabId = ResolveId(page, m_UsedTabIds, tabId);

                foreach (var tab in snipData.Tabs)
                {
                    var tabName = tab.Key;
                    var tabFile = tab.Value;

                    var snipId = ConvertToId(tabFile);

                    snipId = ResolveId(page, m_UsedSnippetIds, snipId);

                    tabsHtml.AppendLine($"<button class=\"tablinks{(isFirst ? " active" : "")}\" onclick=\"openTab(event, '{tabId}', '{snipId}')\">{HttpUtility.HtmlEncode(tabName)}</button>");

                    tabsCode.AppendLine($"<div id=\"{snipId}\" class=\"tabcontent\" style=\"display: {(isFirst ? "block" : "none")}\">");
                    await WriteCodeSnippet(tabsCode, page, tabFile, snipData.Lang,
                        snipData.ExclRegions, snipData.LeftAlign, snipData.Regions);
                    tabsCode.AppendLine("</div>");

                    isFirst = false;
                }

                return string.Format(Resources.code_snippet_tab_container, tabId,
                    tabsHtml.ToString(), tabsCode.ToString());
            }
            else
            {
                var html = new StringBuilder();

                await WriteCodeSnippet(html, page, snipData.FileName, snipData.Lang,
                    snipData.ExclRegions, snipData.LeftAlign, snipData.Regions);

                return html.ToString();
            }
        }

        private string ResolveId(IPage page, Dictionary<IPage, List<string>> usedPageIds, string idCandidate)
        {
            if (!usedPageIds.TryGetValue(page, out List<string> usedIds))
            {
                usedIds = new List<string>();
                usedPageIds.Add(page, usedIds);
            }

            int index = 0;

            var id = idCandidate;

            while (usedIds.Contains(id))
            {
                id = idCandidate + (++index).ToString();
            }

            usedIds.Add(id);
            return id;
        }

        private string ConvertToId(string val) => val
            .Replace(".", "-")
            .Replace("~", "-")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("::", "-")
            .Replace(" ", "-");

        private async Task WriteCodeSnippet(StringBuilder html, IPage page,
            string filePath, string lang, string[] exclRegs, bool leftAlign, string[] regs)
        {
            IAsset snipAsset;

            try
            {
                var searchFolder = FindSnippetFolder(m_Site, page, ref filePath);

                var fileName = new PluginLocation("", Path.GetFileName(filePath), Enumerable.Empty<string>());
                snipAsset = searchFolder.FindAsset(fileName);
            }
            catch (Exception ex)
            {
                throw new NullReferenceException($"Failed to find code snippet: '{filePath}'", ex);
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
                throw new InvalidCastException($"Failed to find an asset at '{filePath}'");
            }
        }

        private Task OnPrePublishFile(ILocation outLoc, PrePublishFileArgs args)
        {
            args.SkipFile = m_Settings.ExcludeSnippets
                && m_SnippetFileIds.Contains(args.File.Id);

            return Task.CompletedTask;
        }

        private Task OnWritePageContent(StringBuilder html, IMetadata data, string url)
        {
            if (html.Length > 0)
            {
                try
                {
                    var writer = new HtmlHeadWriter(html);
                    writer.AddStyleSheets(CSS_FILE_PATH);
                    writer.AddScripts(JS_FILE_PATH);
                    return Task.CompletedTask;
                }
                catch (Exception ex)
                {
                    throw new HeadAssetLinkFailedException(CSS_FILE_PATH, url, ex);
                }
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        private IAssetsFolder FindSnippetFolder(ISite site, IPage page, ref string snipLoc)
        {
            IAssetsFolder snippetsBaseFolder = null;

            if (snipLoc.StartsWith(SNIPPETS_FOLDER_PATH))
            {
                if (m_SnippetsFolder == null)
                {
                    throw new PluginUserMessageException("Snippets folder is not set");
                }

                snipLoc = snipLoc.TrimStart(SNIPPETS_FOLDER_PATH);
                snippetsBaseFolder = m_SnippetsFolder;
            }
            else
            {
                snippetsBaseFolder = AssetsHelper.GetBaseFolder(site, page, PluginLocation.FromPath(snipLoc));
            }

            return snippetsBaseFolder.FindFolder(PluginLocation.FromPath(snipLoc));
        }
    }
}
