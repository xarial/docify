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
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core.Plugin;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompiler : ICompiler
    {
        private readonly ILogger m_Logger;
        private readonly IContentTransformer m_ContentTransformer;
        private readonly BaseCompilerConfig m_Config;
        private readonly ILayoutParser m_LayoutParser;
        private readonly IIncludesHandler m_IncludesHandler;

        [ImportPlugin]
        private IEnumerable<IPreCompilePlugin> m_PreCompilePlugins = null;

        [ImportPlugin]
        private IEnumerable<IPostCompilePlugin> m_PostCompilePlugins = null;

        public BaseCompiler(BaseCompilerConfig config,
            ILogger logger, ILayoutParser layoutParser,
            IIncludesHandler includesHandler,
            IContentTransformer contentTransformer) 
        {
            m_Config = config;
            m_Logger = logger;
            m_LayoutParser = layoutParser;
            m_ContentTransformer = contentTransformer;
            m_IncludesHandler = includesHandler;
        }

        public async Task Compile(Site site)
        {
            m_PreCompilePlugins.InvokePluginsIfAny(p => p.PreCompile(site));

            var allPages = site.GetAllPages();
            var allAssets = site.MainPage.Assets.OfType<TextAsset>();

            if (m_Config.ParallelPartitionsCount == (int)BaseCompilerConfig.ParallelPartitions_e.NoParallelism)
            {
                foreach (var page in allPages)
                {
                    await CompilePage(page, site);
                }

                foreach (var asset in allAssets) 
                {
                    await CompileAsset(asset, site);
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
                await ForEachAsync(allAssets, async a => await CompileAsset(a, site), partitionsCount);
            }

            m_PostCompilePlugins.InvokePluginsIfAny(p => p.PostCompile(site));
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

            var content = await m_ContentTransformer.Transform(page.RawContent, page.Key, model);
            
            var layout = page.Layout;

            if (layout != null)
            {
                content = await m_LayoutParser.InsertContent(layout, content, model);
            }

            content = await m_IncludesHandler.ReplaceAll(content, site, page);

            page.Content = content;
        }

        private async Task CompileAsset(TextAsset asset, Site site)
        {
            var model = new ContextModel(site, null);

            var content = await m_IncludesHandler.ReplaceAll(asset.RawContent, site, null);

            asset.Content = content;
        }
    }
}
