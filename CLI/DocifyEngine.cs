//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Core.Logger;
using Xarial.Docify.Core.Plugin;
using Xarial.Docify.Core.Publisher;
using Xarial.Docify.Lib.Tools;

namespace Xarial.Docify.CLI
{
    public interface IDocifyEngine 
    {
        T Resove<T>();
        Task Build();
    }

    public class DocifyEngine : IDocifyEngine
    {
        private readonly IContainer m_Container;

        private readonly string m_SiteUrl;
        private readonly string m_SrcDir;
        private readonly string m_OutDir;

        public DocifyEngine(string srcDir, string outDir, string siteUrl, Environment_e env)
        {
            var builder = new ContainerBuilder();
            m_SiteUrl = siteUrl;
            m_SrcDir = srcDir;
            m_OutDir = outDir;

            builder.RegisterType<LocalFileSystemLoaderConfig>()
                .UsingConstructor(typeof(Configuration));

            builder.RegisterType<BaseCompilerConfig>()
                .WithParameter(new TypedParameter(typeof(string), ""));

            builder.RegisterType<LocalFileSystemPublisherConfig>();

            builder.RegisterType<LocalFileSystemPublisher>()
                .As<IPublisher>();

            builder.RegisterType<LocalFileSystemLoader>()
                .As<ILoader>();

            builder.RegisterType<LayoutParser>()
                .As<ILayoutParser>();

            builder.RegisterType<BaseSiteComposer>().As<IComposer>();

            builder.RegisterType<ConsoleLogger>().As<ILogger>();

            builder.RegisterType<LocalFileSystemComponentsLoader>().As<IComponentsLoader>();

            builder.RegisterType<RazorLightContentTransformer>();
            builder.RegisterType<MarkdigMarkdownContentTransformer>();

            builder.RegisterType<IncludesHandler>().As<IIncludesHandler>().WithParameter(
                new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IContentTransformer),
                    (pi, ctx) => ctx.Resolve<RazorLightContentTransformer>()))
                .SingleInstance();

            builder.RegisterType<MarkdigRazorLightTransformer>().As<IContentTransformer>()
                .SingleInstance();

            builder.RegisterType<LocalFileSystemConfigurationLoader>().As<IConfigurationLoader>()
                .WithParameter(new TypedParameter(typeof(Environment_e), env));

            builder.RegisterType<BaseCompiler>().As<ICompiler>();

            builder.Register(c => c.Resolve<IConfigurationLoader>().Load(Location.FromPath(m_SrcDir)).Result);

            builder.RegisterType<LocalFileSystemPluginsManager>().As<IPluginsManager>();

            m_Container = builder.Build();

            LoadPlugins();

            MarkdownHelper.MarkdownTransformer = m_Container.Resolve<MarkdigMarkdownContentTransformer>();
        }

        public async Task Build()
        {
            var loader = Resove<ILoader>();
            var composer = Resove<IComposer>();
            var compiler = Resove<ICompiler>();
            var publisher = Resove<IPublisher>();

            var srcFiles = await loader.Load(Location.FromPath(m_SrcDir));

            var compsLoader = Resove<IComponentsLoader>();
            srcFiles = await compsLoader.Load(srcFiles);

            var site = composer.ComposeSite(srcFiles, m_SiteUrl);

            await compiler.Compile(site);

            var writables = Enumerable.Empty<IWritable>();
            writables = writables.Union(site.GetAllPages());
            writables = writables.Union(site.GetAllPages().SelectMany(p => p.Assets));

            await publisher.Write(Location.FromPath(m_OutDir), writables);
        }

        public T Resove<T>() 
        {
            return m_Container.Resolve<T>();
        }

        private void LoadPlugins()
        {
            var plugMgr = m_Container.Resolve<IPluginsManager>();

            foreach (var reg in m_Container.ComponentRegistry.Registrations)
            {
                reg.Activated += (o, eventArgs) =>
                {
                    if (plugMgr != null)
                    {
                        plugMgr.LoadPlugins(eventArgs.Instance);
                    }
                };
            }
        }
    }
}
