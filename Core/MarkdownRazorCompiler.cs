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
        
        public List<Asset> Assets { get; }
        public Page MainPage { get; }
        public List<Template> Layouts { get; }
        public List<Template> Includes { get; }

        public Site(string baseUrl, Page mainPage) 
        {
            BaseUrl = baseUrl;
            MainPage = mainPage;
            Assets = new List<Asset>();
            Layouts = new List<Template>();
            Includes = new List<Template>();
        }
    }

    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public class Page 
    {
        public Location Location { get; }

        public Template Layout { get; }

        public Dictionary<string, string> Data { get; }
        
        public string Content { get; internal set; }

        public List<Page> Children { get; }

        public List<Asset> Assets { get; }
                
        public string RawContent { get; }
        
        public Page(Location url, string rawContent, Template layout = null) 
            : this(url, rawContent, new Dictionary<string, string>(), layout)
        {
            
        }

        public Page(Location url, string rawContent, Dictionary<string, string> data, Template layout = null) 
        {
            Location = url;
            Data = data ?? new Dictionary<string, string>();
            Children = new List<Page>();
            Assets = new List<Asset>();
            RawContent = rawContent;
            Layout = layout;
        }
    }

    public class Asset 
    {
        public string Path { get; }
        public byte[] Content { get; }
    }

    public class Template 
    {
        public string Name { get; }
        public string RawContent { get; }

        public Template(string name, string rawContent) 
        {
            Name = name;
            RawContent = rawContent;
        }
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
            
            foreach (var page in new Page[] { site.MainPage }.Union(site.MainPage.Children.SelectMany(p => p.Children)))
            {
                var model = new RazorModel(site, page);

                var html = page.RawContent;

                if (HasRazorCode(page))
                {
                    html = await razorEngine.CompileRenderAsync(
                        page.Location.ToId(), html, model, typeof(RazorModel));
                }

                //NOTE: by some reasons extra new line symbol is added to the output
                html = Markdown.ToHtml(html, markdownEngine).Trim('\n');

                //page.Layout

                page.Content = html;
            }
        }

        private bool HasRazorCode(Page page) 
        {
            //TODO: might need to have better logic to identify this
            return page.Content.Contains('@');
        }
    }
}
