//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Autofac;
using Autofac.Builder;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Data;
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

    public static class AutoFacExtension
    {
        private const string IMPORT_SERVICE_TO_PLUGINS = "import_service_to_plugins";

        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> ImportServiceToPlugins<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> regBuilder)
        {
            regBuilder.WithMetadata(IMPORT_SERVICE_TO_PLUGINS, true);
            return regBuilder;
        }

        public static bool IsImportServiceToPlugins(this IComponentRegistration compReg) 
        {
            object importService;
            
            if (compReg.Metadata.TryGetValue(IMPORT_SERVICE_TO_PLUGINS, out importService)) 
            {
                if (importService is bool) 
                {
                    return (bool)importService;
                }
            }

            return false;
        }
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

            RegisterDependencies(builder, env);

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

            var srcFiles = loader.Load(Location.FromPath(m_SrcDir));

            var compsLoader = Resove<IComponentsLoader>();
            srcFiles = compsLoader.Load(srcFiles);

            var site = await composer.ComposeSite(srcFiles, m_SiteUrl);

            var writables = compiler.Compile(site);
            
            await publisher.Write(Location.FromPath(m_OutDir), writables);
        }

        public T Resove<T>() 
        {
            return m_Container.Resolve<T>();
        }

        protected virtual void RegisterDependencies(ContainerBuilder builder, Environment_e env) 
        {
            builder.RegisterType<LocalFileSystemLoaderConfig>()
                .UsingConstructor(typeof(IConfiguration));

            builder.RegisterType<BaseCompilerConfig>()
                .WithParameter(new TypedParameter(typeof(string), ""));

            builder.RegisterType<LocalFileSystemPublisherConfig>();

            builder.RegisterType<LocalFileSystemPublisher>()
                .As<IPublisher>()
                .ImportServiceToPlugins();

            builder.RegisterType<LocalFileSystemLoader>()
                .As<ILoader>()
                .ImportServiceToPlugins();

            builder.RegisterType<LayoutParser>()
                .As<ILayoutParser>()
                .ImportServiceToPlugins();

            builder.RegisterType<BaseSiteComposer>().As<IComposer>()
                .ImportServiceToPlugins();

            builder.RegisterType<ConsoleLogger>().As<ILogger>()
                .ImportServiceToPlugins();

            builder.RegisterType<LocalFileSystemComponentsLoader>().As<IComponentsLoader>()
                .ImportServiceToPlugins();

            builder.RegisterType<RazorLightContentTransformer>();

            builder.RegisterType<MarkdigMarkdownContentTransformer>();

            builder.RegisterType<IncludesHandler>().As<IIncludesHandler>().WithParameter(
                new ResolvedParameter(
                    (pi, ctx) => pi.ParameterType == typeof(IContentTransformer),
                    (pi, ctx) => ctx.Resolve<RazorLightContentTransformer>()))
                .SingleInstance()
                .ImportServiceToPlugins();

            builder.RegisterType<MarkdigRazorLightTransformer>().As<IContentTransformer>()
                .SingleInstance()
                .ImportServiceToPlugins();

            builder.RegisterType<LocalFileSystemConfigurationLoader>().As<IConfigurationLoader>()
                .WithParameter(new TypedParameter(typeof(Environment_e), env))
                .ImportServiceToPlugins();

            builder.RegisterType<BaseCompiler>().As<ICompiler>()
                .ImportServiceToPlugins();

            builder.Register(c => c.Resolve<IConfigurationLoader>().Load(Location.FromPath(m_SrcDir)).Result);

            builder.RegisterType<LocalFileSystemPluginsManager>().As<IPluginsManager>();
        }

        private void LoadPlugins()
        {
            var plugMgr = m_Container.Resolve<IPluginsManager>();

            foreach (var reg in m_Container.ComponentRegistry.Registrations)
            {
                //TODO: activated is called on all resolves which makes duplicate calls for the same reference
                reg.Activated += (o, eventArgs) =>
                {
                    if (plugMgr != null)
                    {
                        plugMgr.LoadPlugins(eventArgs.Instance,
                            eventArgs.Component.IsImportServiceToPlugins());
                    }
                };
            }
        }
    }
}
