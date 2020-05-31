//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Autofac;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Core.Logger;
using Xarial.Docify.Core.Plugin;
using Xarial.Docify.Core.Plugin.Extensions;
using Xarial.Docify.Core.Publisher;

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
        private readonly ILocation[] m_SrcDirs;
        private readonly ILocation m_OutDir;
        private readonly ILocation m_LibLoc;

        public DocifyEngine(string[] srcDirs, string outDir, string libPath, string siteUrl, string env)
        {
            var builder = new ContainerBuilder();

            m_SiteUrl = siteUrl;
            m_SrcDirs = srcDirs.Select(s => Location.FromPath(s)).ToArray();
            m_OutDir = Location.FromPath(outDir);

            if (!Path.IsPathRooted(libPath))
            {
                libPath = Path.Combine(Path.GetDirectoryName(typeof(DocifyEngine).Assembly.Location), libPath);
            }

            m_LibLoc = Location.FromPath(libPath);

            RegisterDependencies(builder, env);

            m_Container = builder.Build();
        }

        public async Task Build()
        {
            var loader = Resove<IProjectLoader>();
            var composer = Resove<IComposer>();
            var compiler = Resove<ICompiler>();
            var publisher = Resove<IPublisher>();

            var srcFiles = loader.Load(m_SrcDirs);

            var site = await composer.ComposeSite(srcFiles, m_SiteUrl);

            var outFiles = compiler.Compile(site);

            await publisher.Write(m_OutDir, outFiles);
        }

        public T Resove<T>()
        {
            return m_Container.Resolve<T>();
        }

        protected virtual void RegisterDependencies(ContainerBuilder builder, string env)
        {
            builder.RegisterType<LibraryLoader>()
                .As<ILibraryLoader>()
                .WithParameter(new TypedParameter(typeof(ILocation), m_LibLoc));

            builder.RegisterType<BaseCompilerConfig>()
                .UsingConstructor(typeof(IConfiguration));

            builder.RegisterType<LocalFileSystemPublisher>()
                .As<IPublisher>();

            builder.RegisterType<LocalFileSystemFileLoader>()
                .As<IFileLoader>();

            builder.RegisterType<LayoutParser>()
                .As<ILayoutParser>();

            builder.RegisterType<BaseSiteComposer>().As<IComposer>();

            builder.RegisterType<ConsoleLogger>().As<ILogger>();

            builder.RegisterType<ProjectLoader>().As<IProjectLoader>();

            //NOTE: need this to be single instance to maximize performance and reuse precompiled templates
            builder.RegisterType<RazorLightContentTransformer>()
                .As<IDynamicContentTransformer>()
                .SingleInstance();

            builder.RegisterType<MarkdigMarkdownContentTransformer>()
                .As<IStaticContentTransformer>();

            builder.RegisterType<IncludesHandler>().As<IIncludesHandler>();

            builder.RegisterType<ConfigurationLoader>().As<IConfigurationLoader>()
                .WithParameter(new TypedParameter(typeof(string), env));

            builder.RegisterType<BaseCompiler>().As<ICompiler>();

            builder.Register(c => c.Resolve<IConfigurationLoader>().Load(m_SrcDirs).Result)
                .SingleInstance();

            builder.RegisterType<PluginsManager>().As<IPluginsManager>();

            RegisterExtensions(builder);
        }

        protected virtual void RegisterExtensions(ContainerBuilder builder)
        {
            builder.RegisterType<IncludesHandlerExtension>()
                .SingleInstance()
                .AsSelf()
                .As<IIncludesHandlerExtension>();

            builder.RegisterType<IncludesHandlerManager>().As<IIncludesHandlerManager>();

            builder.RegisterType<CompilerExtension>()
                .SingleInstance()
                .AsSelf()
                .As<ICompilerExtension>();

            builder.RegisterType<CompilerManager>().As<ICompilerManager>();

            builder.RegisterType<ComposerExtension>()
                .SingleInstance()
                .AsSelf()
                .As<IComposerExtension>();

            builder.RegisterType<ComposerManager>().As<IComposerManager>();

            builder.RegisterType<PublisherExtension>()
                .SingleInstance()
                .AsSelf()
                .As<IPublisherExtension>();

            builder.RegisterType<PublisherManager>().As<IPublisherManager>();

            builder.RegisterType<DocifyApplication>().As<IDocifyApplication>();
        }

        //private void LoadPlugins()
        //{
        //    var plugMgr = m_Container.Resolve<IPluginsManager>();
        //    plugMgr.LoadPlugins();
        //}
    }
}
