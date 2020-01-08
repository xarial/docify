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
using System.Collections;

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

    public interface ICompilable 
    {
        string RawContent { get; }
        string Content { get;  set; }
        string Key { get; }
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
        public List<Page> Children { get; }

        public List<Asset> Assets { get; }
        public Location Location { get; }

        public override string Key => Location.ToId();

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

    public abstract class Asset
    {
        public Location Location { get; }
    }

    public abstract class TextAsset : Asset, ICompilable
    {
        public string RawContent { get; }
        public string Content { get; set; }
        public string Key => Location.ToId();
    }

    public abstract class BinaryAsset : Asset
    {
        public byte[] Content { get; }
    }

    public abstract class Frame : ICompilable
    {
        public string RawContent { get; }
        public string Content { get; set; }

        public Template Layout { get; }
        public Dictionary<string, dynamic> Data { get; }

        public abstract string Key { get; }

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
        public override string Key => Name;

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
                await CompileResource(page, new RazorModel(site, page),
                    markdownEngine, razorEngine);
            }
        }

        private async Task CompileResource(ICompilable compilable, RazorModel model,
            MarkdownPipeline markdownEngine, RazorLightEngine razorEngine) 
        {
            var html = compilable.RawContent;

            if (HasRazorCode(compilable))
            {
                html = await razorEngine.CompileRenderAsync(
                    compilable.Key, html, model, model?.GetType());
            }

            //NOTE: by some reasons extra new line symbol is added to the output
            html = Markdown.ToHtml(html, markdownEngine).Trim('\n');

            compilable.Content = html;
        }

        private bool HasRazorCode(ICompilable page) 
        {
            //TODO: might need to have better logic to identify this
            return page.RawContent?.Contains('@') == true;
        }
    }
}
