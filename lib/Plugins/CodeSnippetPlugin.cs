//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Lib.Plugins.Attributes;
using Xarial.Docify.Lib.Plugins.Data;
using Xarial.Docify.Lib.Plugins.Helpers;
using Xarial.Docify.Lib.Plugins.Properties;

namespace Xarial.Docify.Lib.Plugins
{
    public class CodeSnippetSettings
    {
        public string SnippetsFolder { get; set; } = "";
        public bool ExcludeSnippets { get; set; } = true;
    }

    public class CodeSnippetData
    {
        public string FileName { get; set; }
        public Dictionary<string, string> Tabs { get; set; }
        public string[] Regions { get; set; }
        public string[] ExclRegions { get; set; }
        public bool LeftAlign { get; set; } = true;
        public string Lang { get; set; }
    }

    [Plugin("code-snippet")]
    public class CodeSnippetPlugin : IPlugin<CodeSnippetSettings>
    {
        private CodeSnippetSettings m_Settings;

        private const string CSS_FILE_PATH = "assets/styles/code-snippet.css";
        private const char SNIPPETS_FOLDER_PATH = '~';

        private IAssetsFolder m_SnippetsFolder;
        private List<string> m_SnippetFileIds;

        private ISite m_Site;

        private IDocifyApplication m_Engine;

        public void Init(IDocifyApplication engine, CodeSnippetSettings setts)
        {
            m_Engine = engine;
            m_Settings = setts;

            m_Engine.Compiler.PreCompile += OnPreCompile;
            m_Engine.Includes.RegisterCustomIncludeHandler("code-snippet", InsertCodeSnippet);
            m_Engine.Publisher.PrePublishFile += OnPrePublishFile;
            m_Engine.Compiler.WritePageContent += OnWritePageContent;
        }

        private Task OnPreCompile(ISite site)
        {
            m_Site = site;

            AssetsHelper.AddTextAsset(Resources.code_snippet, site.MainPage, CSS_FILE_PATH);

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

            IAsset snipAsset;

            try
            {
                var fileName = snipData.FileName;

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
                throw new NullReferenceException($"Failed to find code snippet: '{snipData.FileName}'", ex);
            }
            
            if (snipAsset != null)
            {
                if (!m_SnippetFileIds.Contains(snipAsset.Id))
                {
                    m_SnippetFileIds.Add(snipAsset.Id);
                }

                var rawCode = snipAsset.AsTextContent();

                var lang = snipData.Lang;

                if (string.IsNullOrEmpty(lang)) 
                {
                    lang = Path.GetExtension(snipAsset.FileName).TrimStart('.').ToLower();
                }

                var snips = CodeSnippetHelper.Select(rawCode, lang, new CodeSelectorOptions()
                {
                    ExcludeRegions = snipData.ExclRegions,
                    LeftAlign = snipData.LeftAlign,
                    Regions = snipData.Regions
                });

                var res = new StringBuilder();

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
                    res.AppendLine(await m_Engine.Compiler.ContentTransformer.Transform(code, Guid.NewGuid().ToString(), null));
                }

                return res.ToString();
            }
            else 
            {
                throw new InvalidCastException($"Failed to find an asset at '{snipData.FileName}'");
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
            var writer = new HtmlHeadWriter(content);
            writer.AddStyleSheets(CSS_FILE_PATH);
            return Task.FromResult(writer.Content);
        }
    }
}
