//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Common.Data;
using Xarial.Docify.Lib.Plugins.Common.Helpers;
using Yahoo.Yui.Compressor;

namespace Xarial.Docify.Lib.Plugins.ScriptStyleOptimizer
{
    public class ScriptStyleOptimizerPlugin : IPlugin<ScriptStyleOptimizerPluginSettings>
    {
        private const string CSS_LINK_TEMPLATE = "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />\r\n";
        private const string SCRIPT_LINK_TEMPLATE = "<script src=\"{0}\"></script>";

        private IDocifyApplication m_App;
        private ScriptStyleOptimizerPluginSettings m_Setts;

        private List<string> m_UsedScripts;
        private List<string> m_UsedStyles;

        private Dictionary<string, StringBuilder> m_BundlesContent;

        private List<IFile> m_DeferredScripts;
        private List<IFile> m_DeferredStyles;

        public void Init(IDocifyApplication app, ScriptStyleOptimizerPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;

            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Compiler.WritePageContent += OnWritePageContent;
            m_App.Publisher.PostAddPublishFiles += OnPostAddPublishFiles;
            m_App.Publisher.PrePublishFile += OnPrePublishFile;
        }

        private Task OnPreCompile(ISite site)
        {
            m_DeferredScripts = new List<IFile>();
            m_DeferredStyles = new List<IFile>();

            m_UsedScripts = new List<string>();
            m_UsedStyles = new List<string>();

            m_BundlesContent = m_Setts.Bundles?.ToDictionary(
                x => x.Key,
                x => new StringBuilder(),
                StringComparer.CurrentCultureIgnoreCase);

            return Task.CompletedTask;
        }

        private Task OnPrePublishFile(ILocation outLoc, PrePublishFileArgs args)
        {
            var ext = Path.GetExtension(args.File.Location.FileName).ToLower();
            var url = args.File.Location.GetRelative(outLoc).ToUrl();

            var isInScope = m_Setts.AssetsScopePaths?.Any(s => PathHelper.Matches(url, s)) != false;

            var bundle = m_Setts.Bundles.FirstOrDefault(
                    b => b.Value.Contains(url, StringComparer.InvariantCultureIgnoreCase)).Key;

            if (isInScope)
            {
                switch (ext)
                {
                    case ".css":

                        if (m_Setts.MinifyCss)
                        {
                            if (!args.File.Location.FileName
                                .EndsWith(".min.css", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var txt = args.File.AsTextContent();
                                var css = new CssCompressor();
                                if (!string.IsNullOrEmpty(txt))
                                {
                                    var cssComp = css.Compress(txt);
                                    args.File = new PluginFile(cssComp, args.File.Location, args.File.Id);
                                }
                            }
                        }

                        if (m_Setts.DeleteUnusedCss)
                        {
                            m_DeferredStyles.Add(args.File);
                            args.SkipFile = true;
                        }

                        break;

                    case ".js":

                        if (m_Setts.MinifyJs)
                        {
                            if (!args.File.Location.FileName
                                .EndsWith(".min.js", StringComparison.CurrentCultureIgnoreCase))
                            {
                                var txt = args.File.AsTextContent();

                                if (!string.IsNullOrEmpty(txt))
                                {
                                    var js = new JavaScriptCompressor();
                                    var jsComp = js.Compress(txt);
                                    args.File = new PluginFile(jsComp, args.File.Location, args.File.Id);
                                }
                            }
                        }

                        if (m_Setts.DeleteUnusedJs)
                        {
                            m_DeferredScripts.Add(args.File);
                            args.SkipFile = true;
                        }

                        break;
                }
            }

            if (!string.IsNullOrEmpty(bundle))
            {
                m_BundlesContent[bundle].AppendLine(args.File.AsTextContent());
            }

            return Task.CompletedTask;
        }

        private async IAsyncEnumerable<IFile> OnPostAddPublishFiles(ILocation outLoc)
        {
            await Task.CompletedTask;

            foreach (var bundle in m_BundlesContent)
            {
                var parts = bundle.Key.Split(PluginLocation.PathSeparators,
                    StringSplitOptions.RemoveEmptyEntries);

                var dir = parts.Take(parts.Length - 1);
                var fileName = parts.Last();

                yield return new PluginFile(bundle.Value.ToString(), outLoc.Combine(new PluginLocation(fileName, dir)));
            }

            await foreach (var defStyle in RetrieveDeferredAssets(
                m_Setts.DeleteUnusedCss, m_DeferredStyles.ToArray(), m_UsedStyles.ToArray(), outLoc))
            {
                yield return defStyle;
            }

            await foreach (var defScript in RetrieveDeferredAssets(
                m_Setts.DeleteUnusedJs, m_DeferredScripts.ToArray(), m_UsedScripts.ToArray(), outLoc))
            {
                yield return defScript;
            }
        }

        private async IAsyncEnumerable<IFile> RetrieveDeferredAssets(bool deleteUnused,
            IFile[] deferredAssets, string[] usedAssets, ILocation outLoc)
        {
            await Task.CompletedTask;

            foreach (var defAsset in deferredAssets)
            {
                var path = defAsset.Location.GetRelative(outLoc).ToUrl();

                if (!deleteUnused || usedAssets.Contains(path))
                {
                    yield return new PluginFile(defAsset.Content, outLoc.Combine(defAsset.Location), defAsset.Id);
                }
            }
        }

        private Task OnWritePageContent(StringBuilder html, IMetadata data, string url)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html.ToString());

            if (m_Setts.Bundles?.Any() == true)
            {
                var headNode = doc.DocumentNode.SelectSingleNode("//head");

                var scripts = doc.DocumentNode.SelectNodes("//head/script[@src]");

                var styles = doc.DocumentNode.SelectNodes("//head/link[@href][@rel='stylesheet']");

                bool hasChanges = false;

                if (scripts?.Any() == true)
                {
                    ReplaceNodes(scripts, "src", headNode, SCRIPT_LINK_TEMPLATE, out bool hasScriptChanges);
                    hasChanges |= hasScriptChanges;
                }

                if (styles?.Any() == true)
                {
                    ReplaceNodes(styles, "href", headNode, CSS_LINK_TEMPLATE, out bool hasStyleChanges);
                    hasChanges |= hasStyleChanges;
                }

                if (hasChanges)
                {
                    var htmlContent = new StringBuilder();

                    using (var strWriter = new StringWriter(htmlContent))
                    {
                        doc.Save(strWriter);
                    }

                    html.Clear();
                    html.Append(htmlContent.ToString());
                }
            }

            if (m_Setts.DeleteUnusedJs)
            {
                var usedScripts = doc.DocumentNode.SelectNodes("//script[@src]")
                    ?.Select(n => n.Attributes["src"].Value);

                if (usedScripts != null)
                {
                    m_UsedScripts.AddRange(usedScripts.Except(m_UsedScripts));
                }
            }

            if (m_Setts.DeleteUnusedJs)
            {
                var usedStyles = doc.DocumentNode.SelectNodes("//link[@href][@rel='stylesheet']")
                    ?.Select(n => n.Attributes["href"].Value);

                if (usedStyles != null)
                {
                    m_UsedStyles.AddRange(usedStyles.Except(m_UsedStyles));
                }
            }

            return Task.CompletedTask;
        }

        private void ReplaceNodes(IEnumerable<HtmlNode> nodes,
            string bundleLinkAttributeName, HtmlNode parentNode, string nodeTemplate, out bool hasChanges)
        {
            hasChanges = false;

            foreach (var bundle in FindBundles(nodes
                .Where(n => n.Attributes.Contains(bundleLinkAttributeName))
                .Select(n => n.Attributes[bundleLinkAttributeName].Value).ToArray()))
            {
                var scriptNode = HtmlNode.CreateNode(string.Format(nodeTemplate, bundle));
                parentNode.AppendChild(scriptNode);

                var nodesToRemove = m_Setts.Bundles[bundle];
                foreach (var node in nodes
                    .Where(n => nodesToRemove.Contains(n.Attributes[bundleLinkAttributeName].Value,
                    StringComparer.CurrentCultureIgnoreCase)))
                {
                    parentNode.RemoveChild(node);
                }

                hasChanges = true;
            }
        }

        private IEnumerable<string> FindBundles(string[] assets)
        {
            foreach (var bundle in m_Setts.Bundles)
            {
                var intersect = assets.Intersect(bundle.Value);

                if (intersect.OrderBy(x => x).SequenceEqual(bundle.Value.OrderBy(x => x)))
                {
                    yield return bundle.Key;
                }
            }
        }
    }
}
