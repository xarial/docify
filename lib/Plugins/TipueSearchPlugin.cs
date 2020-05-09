using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Data;
using Xarial.Docify.Lib.Plugins.Helpers;
using Xarial.Docify.Lib.Plugins.Properties;

namespace Xarial.Docify.Lib.Plugins
{
    public class PageSearchData
    {
        public string title { get; set; }
        public string text { get; set; }
        public string url { get; set; }
        public string tags { get; set; }
        public string img { get; set; }
        public string note { get; set; }
    }

    public class TipueSearchPluginSettings
    {
        public string PageContentNode { get; set; } = "//body";
        public string SearchPageLayout { get; set; } = "";
        public string SearchPageTitle { get; set; } = "Search Results";
    }

    [Plugin("tipue-search")]
    public class TipueSearchPlugin : IPlugin<TipueSearchPluginSettings>, IPreCompilePlugin, IIncludeResolverPlugin, IPageContentWriterPlugin, IPostCompilePlugin
    {
        private readonly string[] STOP_WORDS = new string[] { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "aren't", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", "could", "couldn't", "did", "didn't", "do", "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", "from", "further", "had", "hadn't", "has", "hasn't", "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "isn't", "it", "it's", "its", "itself", "let's", "me", "more", "most", "mustn't", "my", "myself", "no", "nor", "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our", "ours	ourselves", "out", "over", "own", "same", "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "we'd", "we'll", "we're", "we've", "were", "weren't", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves" };

        private const string SITEMAP_PARAM = "sitemap";
        private const string SEARCH_PARAM = "search";
        private const string TITLE_PARAM = "title";

        private const string SEARCH_PAGE_NAME = "search";

        public string IncludeName => "tipue-search";

        private ISite m_Site;
        private List<PageSearchData> m_SearchIndex;

        private TipueSearchPluginSettings m_Setts;

        public void Init(TipueSearchPluginSettings setts)
        {
            m_Setts = setts;
        }

        public Task<string> ResolveInclude(IMetadata data, IPage page)
        {
            return Task.FromResult(Resources.tipue_search_box);
        }

        public Task PreCompile(ISite site)
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

        private string HtmlToPlainText(string html, string node, out string title)
        {
            var ignoreNodes = new string[]
            {
                "script", "style"
            };

            var res = new StringBuilder();

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

            foreach (var textNode in mainNode.SelectNodes(".//text()")
                .Where(n => !ignoreNodes.Contains(n.ParentNode.Name, StringComparer.CurrentCultureIgnoreCase)))
            {
                var txt = textNode.InnerText;

                if (!string.IsNullOrEmpty(txt))
                {
                    if (!Regex.IsMatch(txt, "^(\\n|\\r|\\r\\n) +$"))
                    {
                        res.Append(WebUtility.HtmlDecode(txt));
                    }
                }
            }

            return res.ToString();
        }

        private string NormalizeText(string rawContent)
        {
            var words = Regex.Split(rawContent, @"(\b[^\s]+\b)");

            return string.Join(' ', words.Select(w => w.Trim())
                .Where(w => !STOP_WORDS.Contains(w, StringComparer.CurrentCultureIgnoreCase)));
        }

        public Task<string> WritePageContent(string content, IMetadata data, string url)
        {
            if ((!data.ContainsKey(SITEMAP_PARAM) || data.GetParameterOrDefault<bool>(SITEMAP_PARAM))
                && (!data.ContainsKey(SEARCH_PARAM) || data.GetParameterOrDefault<bool>(SEARCH_PARAM)))
            {
                var text = HtmlToPlainText(content, m_Setts.PageContentNode, out string title);

                m_SearchIndex.Add(new PageSearchData()
                {
                    title = title,
                    text = NormalizeText(text),
                    url = url
                });
            }

            return Task.FromResult(content);
        }

        public async IAsyncEnumerable<IFile> AddFilesPostCompile()
        {
            //to remove the warning
            await Task.CompletedTask;

            var opts = new System.Text.Json.JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };

            var searchContent = System.Text.Json.JsonSerializer.Serialize(m_SearchIndex, opts).ToString();

            yield return new PluginFile($"var tipuesearch = {{ \"pages\": {searchContent} }};", 
                new PluginLocation("search-content.js", new string[] { SEARCH_PAGE_NAME }));
        }
    }
}
