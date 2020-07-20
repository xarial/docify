using Plugins.Themes.UserGuide.CodeSnippetEditLink;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.CodeSnippet;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;

namespace Plugins.CodeSnippetEditLink
{
    public class CodeSnippetEditLinkPlugin : IPlugin<CodeSnippetEditLinkPluginSettings>
    {
        private IDocifyApplication m_App;
        private CodeSnippetEditLinkPluginSettings m_Setts;
        private Dictionary<IAsset, string> m_Assets;
        private ICodeSnippetPlugin m_CodeSnippetPlugin;

        public void Init(IDocifyApplication app, CodeSnippetEditLinkPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;
            m_Assets = new Dictionary<IAsset, string>();

            if (m_Setts.Enable && !string.IsNullOrEmpty(m_Setts.BaseUrl)) 
            {
                m_CodeSnippetPlugin = app.Plugins.OfType<ICodeSnippetPlugin>().FirstOrDefault();

                if (m_CodeSnippetPlugin != null)
                {
                    m_CodeSnippetPlugin.InsertSnippet += OnInsertSnippet;
                    m_App.Compiler.PreCompile += OnPreCompile;
                }
            }
        }

        private void OnInsertSnippet(IAsset asset, ref string htmlSnippet)
        {
            if (!m_Assets.TryGetValue(asset, out string url)) 
            {
                throw new PluginUserMessageException($"Failed to find the asset url '{asset.FileName}'");
            }

            var snippetUrl = m_Setts.BaseUrl.TrimEnd('/') + url;

            var snippetXml = XDocument.Parse(htmlSnippet, LoadOptions.PreserveWhitespace);
            
            var editSnippetBtn = new XElement("button");
            editSnippetBtn.Add(new XAttribute("class", "snippet-btn"));
            var img = new XElement("img");
            img.SetAttributeValue("src", "/_assets/images/edit-icon.svg");
            img.SetAttributeValue("alt", "Edit Snippet");
            img.SetAttributeValue("width", "16px");
            img.SetAttributeValue("height", "16px");
            editSnippetBtn.SetAttributeValue("onclick", $"window.location.href = '{snippetUrl}'");
            editSnippetBtn.Add(img);

            snippetXml.Root.AddFirst(editSnippetBtn);

            htmlSnippet = snippetXml.ToString();
        }

        private Task OnPreCompile(ISite site)
        {
            ParseAssets(site.MainPage, "/");

            return Task.CompletedTask;
        }

        private void ParseAssets(IAssetsFolder folder, string url)
        {
            foreach (var asset in folder.Assets) 
            {
                m_Assets.Add(asset, url + asset.FileName);
            }

            foreach (var subFolder in folder.Folders) 
            {
                ParseAssets(subFolder, url + subFolder.Name + "/");
            }

            if (folder is IPage) 
            {
                foreach (var subPage in (folder as IPage).SubPages) 
                {
                    ParseAssets(subPage, url + subPage.Name + "/");
                }
            }
        }
    }
}
