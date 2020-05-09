using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
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

    [Plugin("tipue-search")]
    public class TipueSearchPlugin : IPreCompilePlugin, IIncludeResolverPlugin
    {
        private readonly string[] STOP_WORDS = new string[] { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "aren't", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", "could", "couldn't", "did", "didn't", "do", "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", "from", "further", "had", "hadn't", "has", "hasn't", "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "isn't", "it", "it's", "its", "itself", "let's", "me", "more", "most", "mustn't", "my", "myself", "no", "nor", "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our", "ours	ourselves", "out", "over", "own", "same", "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "we'd", "we'll", "we're", "we've", "were", "weren't", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves" };

        public string IncludeName => "tipue-search";

        public Task<string> ResolveInclude(IMetadata data, IPage page)
        {
            return Task.FromResult("");
        }

        //TODO: add tipue-search.cshtml to _includes

        public Task PreCompile(ISite site)
        {
            AssetsHelper.AddAssetsFromZip(Resources.tipue_search, site.MainPage);
            return Task.CompletedTask;
        }

        //string IndexPages()
        //{
        //    var pages = Xarial.Docify.Lib.Tools.PageHelper.GetAllPages(Model.Site.MainPage)
        //        .Where(p => !p.Data.ContainsKey("sitemap") || p.Data.Get<bool>("sitemap"));

        //    var pageSearchData = pages.Select(p => new PageSearchData()
        //    {
        //        title = Xarial.Docify.Lib.Tools.PageHelper.GetTitle(p, null),
        //        text = NormalizeText("p.RawContent"),
        //        url = p.FullUrl
        //    });

        //    var opts = new System.Text.Json.JsonSerializerOptions()
        //    {
        //        IgnoreNullValues = true
        //    };

        //    return System.Text.Json.JsonSerializer.Serialize(pageSearchData, opts).ToString();
        //}

        string NormalizeText(string rawContent)
        {
            var words = System.Text.RegularExpressions.Regex.Split(rawContent, @"(\b[^\s]+\b)");

            return string.Join(' ', words.Select(w => w.Trim()).Where(w => !STOP_WORDS.Contains(w, StringComparer.CurrentCultureIgnoreCase)));
        }
    }
}
