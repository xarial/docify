//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Data;
using Xarial.Docify.Lib.Plugins.Helpers;
using Xarial.Docify.Lib.Plugins.TipueSearch.Properties;

namespace Xarial.Docify.Lib.Plugins.TipueSearch
{
    public class PageSearchData
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string Tags { get; set; }
        public string Img { get; set; }
        public string Note { get; set; }
    }

    public class TipueSearchPluginSettings
    {
        public string PageContentNode { get; set; } = "//body";
        public string SearchPageLayout { get; set; } = "";
        public string SearchPageTitle { get; set; } = "Search Results";
    }

    [Plugin("tipue-search")]
    public class TipueSearchPlugin : IPlugin<TipueSearchPluginSettings>
    {
        private const string SITEMAP_PARAM = "sitemap";
        private const string SEARCH_PARAM = "search";
        private const string TITLE_PARAM = "title";

        private const string SEARCH_PAGE_NAME = "search";
        
        private ISite m_Site;
        private List<PageSearchData> m_SearchIndex;

        private TipueSearchPluginSettings m_Setts;

        private IDocifyApplication m_App;

        public void Init(IDocifyApplication app, TipueSearchPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;
            m_App.Includes.RegisterCustomIncludeHandler("tipue-search", InsertSearchBox);
            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Compiler.WritePageContent += OnWritePageContent;
            m_App.Publisher.PostAddPublishFiles += OnPostAddPublishFiles;
        }
        
        private Task<string> InsertSearchBox(IMetadata data, IPage page)
        {
            return Task.FromResult(Resources.tipue_search_box);
        }

        private Task OnPreCompile(ISite site)
        {
            m_Site = site;
            m_SearchIndex = new List<PageSearchData>();

            AssetsHelper.AddAssetsFromZip(Resources.tipue_search, site.MainPage);

            var data = new PluginMetadata();
            data.Add(SITEMAP_PARAM, false);
            data.Add(SEARCH_PARAM, false);
            data.Add(TITLE_PARAM, m_Setts.SearchPageTitle);

            var content = Resources.search;

            ITemplate layout = null;

            if (!string.IsNullOrEmpty(m_Setts.SearchPageLayout))
            {
                layout = site.Layouts.FirstOrDefault(t => string.Equals(t.Name, m_Setts.SearchPageLayout));

                if (layout == null)
                {
                    throw new NullReferenceException($"Specified layout: {m_Setts.SearchPageLayout} for search page cannot be found");
                }
            }
            else 
            {
                content = $"<!DOCTYPE html><html><head></head><body>{content}</body></html>";
            }

            m_Site.MainPage.SubPages.Add(new PluginPage(SEARCH_PAGE_NAME, content, 
                Guid.NewGuid().ToString(), data, layout));

            return Task.CompletedTask;
        }

        private Task<string> OnWritePageContent(string content, IMetadata data, string url)
        {
            if ((!data.ContainsKey(SITEMAP_PARAM) || data.GetParameterOrDefault<bool>(SITEMAP_PARAM))
                && (!data.ContainsKey(SEARCH_PARAM) || data.GetParameterOrDefault<bool>(SEARCH_PARAM)))
            {
                try
                {
                    var text = HtmlToPlainText(content, m_Setts.PageContentNode, out string title);

                    m_SearchIndex.Add(new PageSearchData()
                    {
                        Title = title,
                        Text = NormalizeText(text),
                        Url = url
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to index page for search at '{url}'", ex);
                }
            }

            return Task.FromResult(content);
        }

        private async IAsyncEnumerable<IFile> OnPostAddPublishFiles(ILocation outLoc)
        {
            //to remove the warning
            await Task.CompletedTask;

            var opts = new JsonSerializerOptions()
            {
                IgnoreNullValues = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var searchContent = JsonSerializer.Serialize(m_SearchIndex, opts).ToString();

            yield return new PluginFile($"var tipuesearch = {{ \"pages\": {searchContent} }};", 
                outLoc.Combine(new PluginLocation("search-content.js", new string[] { SEARCH_PAGE_NAME })));
        }

        private string HtmlToPlainText(string html, string node, out string title)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var mainNode = doc.DocumentNode;

            title = mainNode.SelectSingleNode("//head/title").InnerText;

            if (!string.IsNullOrEmpty(node))
            {
                mainNode = mainNode.SelectSingleNode(node);

                if (mainNode == null)
                {
                    throw new Exception($"Failed to find '{node}'");
                }
            }

            return HtmlHelper.HtmlToPlainText(mainNode);
        }

        private string NormalizeText(string rawContent)
        {
            return string.Join(' ', TextHelper.GetWords(rawContent));
        }
    }
}
