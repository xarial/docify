using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Data;
using Yahoo.Yui.Compressor;

namespace Xarial.Docify.Lib.Plugins
{
    public class Bundle 
    {
        public string BundlePath { get; set; }
        public string[] Files { get; set; }
    }

    public class ScriptStyleOptimizerPluginSettings
    {
        public bool MinifyCss { get; set; }
        public bool MinifyJs { get; set; }

        public Bundle[] Bundles { get; set; }
    }

    [Plugin("script-style-optimizer")]
    public class ScriptStyleOptimizerPlugin : IPlugin<ScriptStyleOptimizerPluginSettings>
    {
        private IDocifyApplication m_App;
        private ScriptStyleOptimizerPluginSettings m_Setts;

        public void Init(IDocifyApplication app, ScriptStyleOptimizerPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;

            m_App.Compiler.WritePageContent += OnWritePageContent;
            m_App.Compiler.AddFilesPostCompile += OnAddFilesPostCompile;
            m_App.Publisher.PrePublishFile += OnPrePublishFile;
        }

        private Task<PrePublishResult> OnPrePublishFile(ILocation outLoc, IFile file)
        {
            var res = new PrePublishResult()
            {
                File = file,
                SkipFile = false
            };

            var ext = Path.GetExtension(file.Location.FileName).ToLower();
            
            switch (ext)
            {
                case ".css":
                    if (m_Setts.MinifyCss)
                    {
                        var css = new CssCompressor();
                        var cssComp = css.Compress(file.AsTextContent());
                        res.File = new PluginFile(cssComp, file.Location, file.Id);
                    }
                    break;

                case ".js":
                    if (m_Setts.MinifyJs)
                    {
                        var js = new JavaScriptCompressor();
                        var jsComp = js.Compress(file.AsTextContent());
                        res.File = new PluginFile(jsComp, file.Location, file.Id);
                    }
                    break;
            }
            
            return Task.FromResult(res);
        }

        private async IAsyncEnumerable<IFile> OnAddFilesPostCompile()
        {
            //TODO: add merged assets
            await Task.CompletedTask;

            if (true) 
            {
                yield return null;
            }
        }
        
        private Task<string> OnWritePageContent(string content, IMetadata data, string url)
        {
            var res = new StringBuilder();

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var scripts = doc.DocumentNode.SelectNodes("//head/script");
            var styles = doc.DocumentNode.SelectNodes("//head/link[@rel='stylesheet']");

            var scriptNode = new HtmlNode(HtmlNodeType.Element, doc, 0);
            scriptNode.Name = "script";
            scriptNode.Attributes.Add("src", "");

            var linkNode = new HtmlNode(HtmlNodeType.Element, doc, 1);
            linkNode.Name = "link";
            linkNode.Attributes.Add("rel", "stylesheet");
            linkNode.Attributes.Add("type", "text/css");
            linkNode.Attributes.Add("href", "");

            //TODO: replace scripts references
            return Task.FromResult(content);
        }
    }
}
