//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;

namespace Xarial.Docify.CLI
{
    public interface IDocifyEngine 
    {
        T Resove<T>();
    }

    public class DocifyEngine : IDocifyEngine
    {
        private IContainer m_Container;

        public DocifyEngine(string srcDir, string outDir) 
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<LocalFileSystemLoaderConfig>()
                .WithParameter(new TypedParameter(typeof(string), srcDir));

            builder.RegisterType<BaseCompilerConfig>()
                .WithParameter(new TypedParameter(typeof(string), ""));

            builder.RegisterType<LocalFileSystemPublisherConfig>()
                .WithParameter(new TypedParameter(typeof(string), outDir));
            
            builder.RegisterType<LocalFileSystemPublisher>()
                .As<IPublisher>();

            builder.RegisterType<LocalFileSystemLoader>()
                .As<ILoader>();

            builder.RegisterType<LayoutParser>()
                .As<ILayoutParser>();

            builder.RegisterType<SiteComposer>().As<IComposer>();

            builder.RegisterType<Logger>().As<ILogger>();

            builder.RegisterType<IncludesHandler>().As<IIncludesHandler>();

            builder.RegisterType<MarkdigRazorLightTransformer>()
                .As<IContentTransformer>()
                .UsingConstructor(typeof(Func<IContentTransformer, IIncludesHandler>));

            builder.RegisterType<BaseCompiler>().As<ICompiler>();

            m_Container = builder.Build();
        }

        public T Resove<T>() 
        {
            return m_Container.Resolve<T>();
        }
    }
}
