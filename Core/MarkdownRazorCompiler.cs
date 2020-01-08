//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************/

using RazorLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Core.Base;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Core.Tests")]

namespace Xarial.Docify.Core
{
    public class RazorModel 
    {
        public Site Site { get; }
        public Page Page { get; }
    }

    public class Site 
    {
        public string BaseUrl { get; }
        public IEnumerable<Asset> Assets { get; }
        public IEnumerable<Page> Pages { get; }

        internal Site(string baseUrl,
            IEnumerable<Asset> assets, IEnumerable<Page> pages) 
        {
            BaseUrl = baseUrl;
            Assets = assets;
            Pages = pages;
        }
    }

    [DebuggerDisplay("{" + nameof(Url) + "}")]
    public class Page 
    {
        public string Url { get; }
        public IReadOnlyDictionary<string, string> Data { get; }
        //[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        //public string Content { get; internal set; }
        public IEnumerable<Page> Children => ChildrenList;
        public IEnumerable<Asset> Assets { get; }
        //IPageSource Source { get; }
        string RawContent { get; }
        internal List<Page> ChildrenList { get; }

        internal Page(string url, IReadOnlyDictionary<string, string> data,
            IEnumerable<Asset> assets, string rawContent)
        {
            Url = url;
            Data = data;
            ChildrenList = new List<Page>();
            Assets = assets;
            RawContent = rawContent;
        }
    }

    public class Asset 
    {
        public string Path { get; }
    }

    public class MarkdownRazorCompilerConfig : ICompilerConfig
    {   
        public string SiteUrl { get; }

        public MarkdownRazorCompilerConfig(string siteUrl) 
        {
            SiteUrl = siteUrl;
        }
    }

    public class MarkdownRazorCompiler : ICompiler
    {
        public ICompilerConfig Configuration => m_Config;

        public ILogger Logger { get; }

        public IPublisher Publisher { get; }

        private readonly MarkdownRazorCompilerConfig m_Config;

        public MarkdownRazorCompiler(MarkdownRazorCompilerConfig config,
            ILogger logger, IPublisher publisher) 
        {
            m_Config = config;
            Logger = logger;
            Publisher = publisher;
        }
        
        public async Task Compile(ISiteSource siteSrc)
        {
            var site = ComposeSite(siteSrc);

            //TODO: build all includes (identify if any circular)
            //TODO: build all layout (identify if any circular)
            //TODO: parallel all pages building
            //TODO: identify if any layouts or includes are not in use

            //var engine = new RazorLightEngineBuilder()
            //  .UseMemoryCachingProvider()
            //  .Build();

            //string template = "Hello, @Model.Name. Welcome to RazorLight repository";
            //var model = new { Name = "John Doe" };

            //string result = await engine.CompileRenderAsync("templateKey", template, model);
        }

        private const char PATH_SEPARATOR = '\\';

        internal Site ComposeSite(ISiteSource src) 
        {
            var pages = new Dictionary<string, Page>(
                StringComparer.CurrentCultureIgnoreCase);

            var rootPages = new List<Page>();

            if (src.Pages != null) 
            {
                //TODO: handle the duplicate key exception and rethrow
                var pagePerRelPath = src.Pages.ToDictionary(
                    p => GetRelativePath(p.Path, src.Path), p => p);

                foreach (var pageData in pagePerRelPath.OrderBy(
                    p => p.Key.Count(c => c.Equals(PATH_SEPARATOR)))) 
                {
                    var relPath = pageData.Key;
                    var pageSrc = pageData.Value;
                    var pathParts = relPath.Split(PATH_SEPARATOR).SkipLast(1).ToArray();

                    var url = "";
                    
                    for (int i = 0; i < pathParts.Length; i++) 
                    {
                        var isRoot = i == 0;
                        var isPage = i == pathParts.Length - 1;

                        var sep = (i != 0) ? "/" : "";
                        var thisUrl = $"{url}{sep}{pathParts[i]}";

                        Page page = null;

                        if (!pages.TryGetValue(thisUrl, out page))
                        {
                            string thisRawContent = null;
                            IReadOnlyDictionary<string, string> thisPageData = null;

                            if (isPage)
                            {
                                GetPageData(pageSrc, out thisPageData, out thisRawContent);
                            }
                            else 
                            {
                                //TODO: implement default attributes and raw content for auto-pages
                            }

                            page = new Page(thisUrl, thisPageData, null, thisRawContent);
                            pages.Add(thisUrl, page);

                            if (isRoot)
                            {
                                rootPages.Add(page);
                            }

                            if (!string.IsNullOrEmpty(url))
                            {
                                pages[url].ChildrenList.Add(page);
                            }
                        }
                        else 
                        {
                            if (isPage) 
                            {
                                throw new Exception("Duplicate page");
                            }
                        }

                        url = thisUrl;
                    }
                }
            }

            return new Site(m_Config.SiteUrl, null, rootPages);
        }

        private void GetPageData(IPageSource pageSrc, out IReadOnlyDictionary<string, string> data, out string rawContent) 
        {
            //TODO: extract front matter
            data = null;
            rawContent = null;
        }

        private bool IsInFolder(string path, string folderPath) 
        {
            return path.StartsWith(folderPath, 
                StringComparison.CurrentCultureIgnoreCase);
        }

        private string GetRelativePath(string path, string basePath) 
        {
            if (IsInFolder(path, basePath))
            {
                path = path.Substring(basePath.Length);
            }

            return path.Trim(PATH_SEPARATOR);
        }
    }
}
