//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Text.RegularExpressions;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompiler : ICompiler
    {
        public ILogger Logger { get; }

        public IPublisher Publisher { get; }
        private readonly IContentTransformer m_ContentTransformer;

        private readonly BaseCompilerConfig m_Config;

        private readonly ILayoutParser m_LayoutParser;

        public BaseCompiler(BaseCompilerConfig config,
            ILogger logger, IPublisher publisher, ILayoutParser layoutParser,
            IContentTransformer contentTransformer) 
        {
            m_Config = config;
            Logger = logger;
            Publisher = publisher;
            m_LayoutParser = layoutParser;
            m_ContentTransformer = contentTransformer;
        }

        public async Task Compile(Site site)
        {
            var allPages = site.MainPage.GetAllPages();

            if (m_Config.ParallelPartitionsCount == (int)BaseCompilerConfig.ParallelPartitions_e.NoParallelism)
            {
                foreach (var page in allPages)
                {
                    await CompilePage(page, site);
                }
            }
            else 
            {
                //this is preview only option as sometimes exception is thrown, perhaps some of the engines are not thread safe
                int partitionsCount = 1;

                switch ((BaseCompilerConfig.ParallelPartitions_e)m_Config.ParallelPartitionsCount)
                {
                    case BaseCompilerConfig.ParallelPartitions_e.Infinite:
                        partitionsCount = -1;
                        break;

                    case BaseCompilerConfig.ParallelPartitions_e.AutoDetect:
                        partitionsCount = Environment.ProcessorCount;
                        break;
                }

                await ForEachAsync(allPages, async p => await CompilePage(p, site), partitionsCount);
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

        private async Task CompilePage(Page page, Site site)
        {
            var model = new ContextModel(site, page);

            var content = await CompileResource(page, model);

            var layout = page.Layout;

            while (layout != null)
            {
                var layoutContent = await CompileResource(layout, model);

                content = m_LayoutParser.InsertContent(layoutContent, content);

                layout = layout.Layout;
            }

            page.Content = content;
        }

        private async Task<string> CompileResource(ICompilable compilable, ContextModel model) 
        {
            var html = await m_ContentTransformer.Transform(compilable.RawContent, compilable.Key, model);

            //TODO: identify if any includes are not in use

            return html;
        }
    }
}
