//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Autofac;
using Autofac.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI.Properties;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Host;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Core.Logger;
using Xarial.Docify.Core.Plugin;
using Xarial.Docify.Core.Plugin.Extensions;
using Xarial.Docify.Core.Publisher;
using Xarial.Docify.Core.Tools;
using Xarial.XToolkit.Services.UserSettings;

namespace Xarial.Docify.CLI
{
    public interface IDocifyEngine
    {
        T Resolve<T>();
        Task Build();
    }

    public interface IDocifyServeEngine
    {
        T Resolve<T>();
        Task Serve(Func<Task> serveCallback);
    }

    public class DocifyServeEngine : DocifyEngine, IDocifyServeEngine
    {
        private static string GetOutDir(string outDir)
        {
            if (string.IsNullOrEmpty(outDir)) 
            {
                outDir = Path.Combine(Path.GetTempPath(), "Docify", Guid.NewGuid().ToString());
            }

            return outDir;
        }

        private readonly int m_HttpPort;
        private readonly int m_HttpsPort;

        public DocifyServeEngine(string[] srcDirs, string outDir, 
            string[] libs, string host, string baseUrl, string env, bool verbose, int httpPort, int httpsPort)
            : base(srcDirs, GetOutDir(outDir), libs, host, baseUrl, env, verbose)
        {
            m_HttpPort = httpPort;
            m_HttpsPort = httpsPort;
        }

        public async Task Serve(Func<Task> serveCallback)
        {
            var host = Resolve<ISiteHost>();
            
            await Build();
                        
            await host.Host(m_OutDir, serveCallback);

            var dirCleaner = Resolve<ITargetDirectoryCleaner>();
            await dirCleaner.ClearDirectory(m_OutDir);
        }

        protected override void RegisterDependencies(ContainerBuilder builder, string env)
        {
            base.RegisterDependencies(builder, env);

            builder.RegisterType<OwinSiteHost>()
                .As<ISiteHost>()
                .WithParameter(
                new ResolvedParameter(
                    (pi, cx) => pi.ParameterType == typeof(HostSettings), 
                    (pi, cx) => new HostSettings(m_HttpPort, m_HttpsPort)));
        }
    }

    public class DocifyEngine : IDocifyEngine
    {
        internal const string STANDARD_LIB_PATH = "*";
        internal const string LIB_PATH_PUBLIC_KEY_SEP = "|";

        private readonly IContainer m_Container;

        private readonly string m_Host;
        private readonly string m_BaseUrl;

        private readonly ILocation[] m_SrcDirs;
        protected readonly ILocation m_OutDir;
        private readonly string[] m_Libs;
        private readonly bool m_Verbose;

        public DocifyEngine(string[] srcDirs, string outDir, string[] libs, string host, string baseUrl, string env, bool verbose)
        {
            var builder = new ContainerBuilder();

            m_Host = host;
            m_BaseUrl = baseUrl;

            if (srcDirs?.Any() != true) 
            {
                srcDirs = new string[] { Directory.GetCurrentDirectory() };
            }

            m_SrcDirs = srcDirs.Select(s => Location.FromPath(s)).ToArray();
            m_OutDir = Location.FromPath(outDir);

            m_Libs = libs;

            m_Verbose = verbose;

            RegisterDependencies(builder, env);

            m_Container = builder.Build();
        }

        public async Task Build()
        {
            var loader = Resolve<IProjectLoader>();
            var composer = Resolve<IComposer>();
            var compiler = Resolve<ICompiler>();
            var publisher = Resolve<IPublisher>();

            var srcFiles = loader.Load(m_SrcDirs);

            var site = await composer.ComposeSite(srcFiles, m_Host, m_BaseUrl);

            var outFiles = compiler.Compile(site);

            await publisher.Write(m_OutDir, outFiles);
        }
        
        public T Resolve<T>()
        {
            return m_Container.Resolve<T>();
        }

        protected virtual void RegisterDependencies(ContainerBuilder builder, string env)
        {
            builder.RegisterType<LibraryLoader>()
                .As<ILibraryLoader>()
                .WithParameter(new ResolvedParameter(
                       (pi, ctx) => pi.ParameterType == typeof(ILibraryLoader[]),
                       (pi, ctx) => ResolveLibraryLoaders(ctx).ToArray()));

            builder.RegisterType<FolderLibraryLoader>();
            builder.RegisterType<SecureLibraryLoader>();

            builder.RegisterType<BaseCompilerConfig>()
                .UsingConstructor(typeof(IConfiguration));

            builder.RegisterType<LocalFileSystemTargetDirectoryCleaner>()
                .As<ITargetDirectoryCleaner>();

            builder.RegisterType<LocalFileSystemPublisher>()
                .As<IPublisher>();

            builder.RegisterType<LocalFileSystemFileLoader>()
                .As<IFileLoader>();

            builder.RegisterType<LayoutParser>()
                .As<ILayoutParser>();

            builder.RegisterType<BaseSiteComposer>().As<IComposer>();

            builder.RegisterType<ConsoleLogger>().As<ILogger>()
                .WithParameter(new TypedParameter(typeof(bool), m_Verbose));

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

            builder.RegisterType<LoaderExtension>()
                .SingleInstance()
                .AsSelf()
                .As<ILoaderExtension>();

            builder.RegisterType<LoaderManager>().As<ILoaderManager>();

            builder.RegisterType<DocifyApplication>().As<IDocifyApplication>();
        }

        private IEnumerable<ILibraryLoader> ResolveLibraryLoaders(IComponentContext ctx)
        {
            if (m_Libs?.Any() == true)
            {
                foreach (var lib in m_Libs)
                {
                    var libData = lib.Split(LIB_PATH_PUBLIC_KEY_SEP);

                    var libPath = libData[0];

                    if (libPath == STANDARD_LIB_PATH)
                    {
                        libPath = Location.Library.DefaultLibraryManifestFilePath.ToPath();

                        ILibraryLoader standardLib;

                        try
                        {
                            standardLib = ResolveSecureLibrary(ctx, libPath, Resources.standard_library_public_key);
                        }
                        catch (FileNotFoundException ex) 
                        {
                            throw new UserMessageException("Standard library is not installed. Use the library --install command to install the library", ex);
                        }

                        yield return standardLib;
                    }
                    else
                    {
                        if (!Path.IsPathRooted(libPath))
                        {
                            libPath = Path.Combine(Path.GetDirectoryName(typeof(DocifyEngine).Assembly.Location), libPath);
                        }

                        var libLoc = Location.FromPath(libPath);

                        if (!libLoc.IsFile())
                        {
                            if (!Directory.Exists(libPath))
                            {
                                throw new UserMessageException($"Specified library folder is not found: {libPath}");
                            }

                            yield return ctx.Resolve<FolderLibraryLoader>(new TypedParameter(typeof(ILocation), libLoc));
                        }
                        else
                        {
                            if (libData.Length > 1)
                            {
                                var publicKeyXmlFilePath = libData[1];

                                if (!System.IO.File.Exists(publicKeyXmlFilePath))
                                {
                                    throw new UserMessageException($"Cannot find the public key XML file at '{publicKeyXmlFilePath}'");
                                }

                                var publicKeyXml = System.IO.File.ReadAllText(publicKeyXmlFilePath);

                                yield return ResolveSecureLibrary(ctx, libPath, publicKeyXml);
                            }
                            else
                            {
                                throw new UserMessageException($"When specifying path '{libPath}' to the library manifest file, the path to public key XML must be specified as well by separating path with pipe {LIB_PATH_PUBLIC_KEY_SEP} symbol or use {STANDARD_LIB_PATH} for the standard library");
                            }
                        }
                    }
                }
            }
        }

        private SecureLibraryLoader ResolveSecureLibrary(IComponentContext ctx, string manifestPath, string publicKeyXml)
        {
            if (!System.IO.File.Exists(manifestPath))
            {
                throw new UserMessageException($"Specified library manifest file is not found: {manifestPath}");
            }

            var manifest = new UserSettingsService().ReadSettings<SecureLibraryManifest>(
                manifestPath, new BaseValueSerializer<ILocation>(null, x => Location.FromString(x)));

            ILocation libLoc = Location.FromString(manifestPath);
            libLoc = libLoc.Create(libLoc.Root, "", libLoc.Segments);

            return ctx.Resolve<SecureLibraryLoader>(
                   new TypedParameter(typeof(ILocation), libLoc),
                   new TypedParameter(typeof(SecureLibraryManifest), manifest),
                   new TypedParameter(typeof(string), publicKeyXml));
        }
    }
}
