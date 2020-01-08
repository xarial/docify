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
    public class Page : Frame
    {
        public Location Location { get; }

        public string Content { get; internal set; }

        public List<Page> Children { get; }

        public List<Asset> Assets { get; }
        
        public Page(Location url, string rawContent, Template layout = null) 
            : this(url, rawContent, new Dictionary<string, dynamic>(), layout)
        {
            
        }

        public Page(Location url, string rawContent, Dictionary<string, dynamic> data, Template layout = null) 
            : base(rawContent, data, layout)
        {
            Location = url;
            Children = new List<Page>();
            Assets = new List<Asset>();
        }
    }

    public class Asset 
    {
        public Location Location { get; }
        public byte[] Content { get; }
    }

    public abstract class Frame 
    {
        public string RawContent { get; }
        public Template Layout { get; }
        public Dictionary<string, dynamic> Data { get; }

        public Frame(string rawContent, Dictionary<string, dynamic> data, Template layout) 
        {
            RawContent = rawContent;
            Layout = layout;
            Data = data ?? new Dictionary<string, dynamic>();
        }
    }

    public class Template : Frame
    {
        public string Name { get; }
        
        public Template(string name, string rawContent,
            Dictionary<string, dynamic> data = null, Template baseTemplate = null) 
            : base(rawContent, data, baseTemplate)
        {
            Name = name;
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

        private void GetAllPages(Page page, List<Page> allPages) 
        {
            allPages.Add(page);

            if (page.Children != null) 
            {
                foreach (var childPage in page.Children)
                {
                    GetAllPages(childPage, allPages);
                }
            }
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

            var allPages = new List<Page>();
            GetAllPages(site.MainPage, allPages);

            foreach (var page in allPages)
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

                page.Content = html;
            }
        }

        private bool HasRazorCode(Page page) 
        {
            //TODO: might need to have better logic to identify this
            return page.RawContent?.Contains('@') == true;
        }
    }
}
