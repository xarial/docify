//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

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
using Markdig;

namespace Xarial.Docify.Core
{
    public class RazorModel 
    {
        public Site Site { get; }
        public Page Page { get; }

        public RazorModel(Site site, Page page) 
        {
            Site = site;
            Page = page;
        }
    }

    public class Site 
    {
        public string BaseUrl { get; }
        public IEnumerable<Asset> Assets { get; }
        public IEnumerable<Page> Pages { get; }

        public Site(string baseUrl,
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
        
        public string Content { get; internal set; }

        public IEnumerable<Page> Children => ChildrenList;

        public IEnumerable<Asset> Assets { get; }
        //IPageSource Source { get; }
        
        public string RawContent { get; }
        
        internal List<Page> ChildrenList { get; }

        public Page(string url, IReadOnlyDictionary<string, string> data,
            IEnumerable<Asset> assets, string rawContent)
        {
            Url = url;
            Data = data;
            ChildrenList = new List<Page>();
            Assets = assets;
            RawContent = rawContent;
        }

        public Page(string url, IReadOnlyDictionary<string, string> data,
            IEnumerable<Asset> assets, string rawContent, List<Page> children)
            : this(url, data, assets, rawContent)
        {
            ChildrenList = children;
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
        
        public async Task Compile(Site site)
        {
            //TODO: build all includes (identify if any circular)
            //TODO: build all layout (identify if any circular)
            //TODO: parallel all pages building
            //TODO: identify if any layouts or includes are not in use

            var razorEngine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();

            var markdownEngine = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                //.UseSyntaxHighlighting() //requires Markdig.SyntaxHighlighting
                .Build();
            
            foreach (var page in site.Pages.Union(site.Pages.SelectMany(p => p.Children)))
            {
                var model = new RazorModel(site, page);

                var html = await razorEngine.CompileRenderAsync(
                    page.Url, page.RawContent, model, typeof(RazorModel));

                html = Markdown.ToHtml(html, markdownEngine).Trim('\n');

                page.Content = html;
            }
        }
    }
}
