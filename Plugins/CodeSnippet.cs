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
using Xarial.Docify.Lib.Plugins.Properties;

namespace Xarial.Docify.Lib.Plugins
{
    public class CodeSnippetSettings
    {
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
    public class CodeSnippet : IPreCompilePlugin, IRenderIncludePlugin, IPrePublishFilePlugin, IPlugin<CodeSnippetSettings>
    {
        public string IncludeName => "code-snippet";

        private CodeSnippetSettings m_Settings;

        private const string CSS_FILE_NAME = "code-snippet.css";
        private readonly string[] CSS_FILE_PATH = new string[] { "assets", "styles" };

        private List<ILocation> m_SnippetFiles;

        [ImportService]
        private IContentTransformer m_ContentTransformer;

        public void Init(CodeSnippetSettings setts)
        {
            m_Settings = setts;
        }

        public void PreCompile(ISite site)
        {
            site.MainPage.Assets.Add(new PluginDataFile(Resources.code_snippet,
                new PluginDataFileLocation(CSS_FILE_NAME, CSS_FILE_PATH)));

            m_SnippetFiles = new List<ILocation>();
        }

        public async Task<string> GetContent(IMetadata data, IPage page)
        {
            var snipData = data.ToObject<CodeSnippetData>();

            var snipAsset = page.Assets.FirstOrDefault(a => string.Equals(
                a.Location.FileName, snipData.FileName, StringComparison.CurrentCultureIgnoreCase));

            if (snipAsset != null)
            {
                m_SnippetFiles.Add(snipAsset.Location);

                var rawCode = snipAsset.AsTextContent();

                var lang = snipData.Lang;

                if (string.IsNullOrEmpty(lang)) 
                {
                    lang = Path.GetExtension(snipAsset.Location.FileName).TrimStart('.').ToLower();
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

                    switch (snip.Info) 
                    {
                        case SnippetInfo_e.Jagged:
                            snipClass = "jagged";
                            break;

                        case SnippetInfo_e.BottomJagged:
                            snipClass = "bottom-jagged";
                            break;

                        case SnippetInfo_e.TopJagged:
                            snipClass = "top-jagged";
                            break;
                    }

                    var code = $"~~~{lang} {snipClass}\r\n{snip.Code}\r\n~~~";
                    res.AppendLine(await m_ContentTransformer.Transform(code, Guid.NewGuid().ToString(), null));
                }

                return res.ToString();
            }
            else 
            {
                throw new InvalidCastException($"Failed to find an asset at '{snipData.FileName}'");
            }
        }

        public void PrePublishFile(ILocation outLoc, ref IFile file, out bool skip)
        {
            if (string.Equals(Path.GetExtension(file.Location.FileName), ".html", StringComparison.InvariantCultureIgnoreCase))
            {
                var pageContent = file.AsTextContent();
                Helper.InjectDataIntoHtmlHead(ref pageContent,
                    string.Format(Helper.CSS_LINK_TEMPLATE, string.Join('/', CSS_FILE_PATH) + "/" + CSS_FILE_NAME));
                file = new PluginDataFile(pageContent, file.Location);
                skip = false;
            }
            else 
            {
                skip = m_Settings.ExcludeSnippets && m_SnippetFiles.Contains(file.Location);
            }
        }
    }
}
