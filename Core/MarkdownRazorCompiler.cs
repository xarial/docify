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
using System.Text.RegularExpressions;

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
        public string Content { get; set; }

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

        public Asset(Location loc) 
        {
            Location = loc;
        }
    }

    public class TextAsset : Asset, ICompilable
    {
        public string RawContent { get; }
        public string Content { get; set; }
        public string Key => Location.ToId();

        public TextAsset(string rawContent, Location loc) : base(loc) 
        {
            RawContent = rawContent;
        }
    }

    public class BinaryAsset : Asset
    {
        public byte[] Content { get; }

        public BinaryAsset(byte[] content, Location loc) : base(loc)
        {
            Content = content;
        }
    }

    public abstract class Frame : ICompilable
    {
        public string RawContent { get; }
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

    public interface ILayoutParser
    {
        string PlaceholderValue { get; }
        bool ContainsPlaceholder(string content);
        string InsertContent(string content, string insertContent);
    }

    public class LayoutParser : ILayoutParser
    {
        private const string CONTENT_PLACEHOLDER_REGEX = "{{ *content *}}";

        public string PlaceholderValue => CONTENT_PLACEHOLDER_REGEX;

        public bool ContainsPlaceholder(string content) 
        {
            return Regex.IsMatch(content, CONTENT_PLACEHOLDER_REGEX);
        }

        public string InsertContent(string content, string insertContent)
        {
            return Regex.Replace(content, CONTENT_PLACEHOLDER_REGEX, insertContent);
        }
    }
    
    public class MarkdownRazorCompilerConfig : ICompilerConfig
    {
        public enum ParallelPartitions_e 
        {
            Infinite = -1,
            AutoDetect = 0,
            NoParallelism = 1
        }

        public string SiteUrl { get; }

        /// <summary>
        /// Number of partitions for parallel job. See <see cref="ParallelPartitions_e"/> for options
        /// </summary>
        public int ParallelPartitionsCount { get; set; }

        public MarkdownRazorCompilerConfig(string siteUrl) 
        {
            SiteUrl = siteUrl;
            ParallelPartitionsCount = (int)ParallelPartitions_e.NoParallelism;
        }
    }

    public class MarkdownRazorCompiler : ICompiler
    {
        public ICompilerConfig Configuration => m_Config;

        public ILogger Logger { get; }

        public IPublisher Publisher { get; }

        private readonly MarkdownRazorCompilerConfig m_Config;

        private readonly ILayoutParser m_LayoutParser;

        public MarkdownRazorCompiler(MarkdownRazorCompilerConfig config,
            ILogger logger, IPublisher publisher, ILayoutParser layoutParser) 
        {
            m_Config = config;
            Logger = logger;
            Publisher = publisher;
            m_LayoutParser = layoutParser;
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
            var razorEngine = new RazorLightEngineBuilder()
                .UseMemoryCachingProvider()
                .Build();

            var markdownEngine = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                //.UseSyntaxHighlighting() //requires Markdig.SyntaxHighlighting
                .Build();

            var allPages = new List<Page>();
            GetAllPages(site.MainPage, allPages);

            if (m_Config.ParallelPartitionsCount == (int)MarkdownRazorCompilerConfig.ParallelPartitions_e.NoParallelism)
            {
                foreach (var page in allPages)
                {
                    await CompilePage(page, site,
                        markdownEngine, razorEngine);
                }
            }
            else 
            {
                //this is preview only option as sometimes exception is thrown, perhaps some of the engines are not thread safe
                int partitionsCount = 1;

                switch ((MarkdownRazorCompilerConfig.ParallelPartitions_e)m_Config.ParallelPartitionsCount)
                {
                    case MarkdownRazorCompilerConfig.ParallelPartitions_e.Infinite:
                        partitionsCount = -1;
                        break;

                    case MarkdownRazorCompilerConfig.ParallelPartitions_e.AutoDetect:
                        partitionsCount = Environment.ProcessorCount;
                        break;
                }

                await ForEachAsync(allPages, async p => await CompilePage(p, site,
                        markdownEngine, razorEngine), partitionsCount);
            }
        }

        private Task ForEachAsync<T>(IEnumerable<T> source, Func<T, Task> body, int partitionsCount)
        {
            if (partitionsCount == -1) 
            {
                partitionsCount = source.Count();
            }

            return Task.WhenAll(
                System.Collections.Concurrent.Partitioner.Create(source).GetPartitions(partitionsCount)
                .Select(partition => Task.Run(async delegate
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        {
                            await body(partition.Current);
                        }
                    }
                })));
        }

        private async Task CompilePage(Page page, Site site,
            MarkdownPipeline markdownEngine, RazorLightEngine razorEngine)
        {
            var model = new RazorModel(site, page);

            var content = await CompileResource(page, model,
                    markdownEngine, razorEngine);

            var layout = page.Layout;

            while (layout != null)
            {
                var layoutContent = await CompileResource(layout, model,
                    markdownEngine, razorEngine);

                content = m_LayoutParser.InsertContent(layoutContent, content);

                layout = layout.Layout;
            }

            page.Content = content;
        }

        private async Task<string> CompileResource(ICompilable compilable, RazorModel model,
            MarkdownPipeline markdownEngine, RazorLightEngine razorEngine) 
        {
            var html = compilable.RawContent;

            if (HasRazorCode(compilable))
            {
                html = await razorEngine.CompileRenderAsync(
                    compilable.Key, html, model, model?.GetType());
            }

            //TODO: identify if any includes are not in use

            //NOTE: by some reasons extra new line symbol is added to the output
            html = Markdown.ToHtml(html, markdownEngine).Trim('\n');
            
            return html;
        }

        private bool HasRazorCode(ICompilable page) 
        {
            //TODO: might need to have better logic to identify this
            return page.RawContent?.Contains('@') == true;
        }
    }
}
