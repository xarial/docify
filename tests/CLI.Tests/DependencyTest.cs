//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;

namespace CLI.Tests
{
    public class DependencyTest
    {
        [Test]
        public void ResolveTest() 
        {
            var engine = new DocifyEngineMock();

            Assert.DoesNotThrow(() => engine.Resolve<ICompiler>(), "ICompiler");
            Assert.DoesNotThrow(() => engine.Resolve<IFileLoader>(), "IFileLoader");
            Assert.DoesNotThrow(() => engine.Resolve<IComposer>(), "IComposer");
            Assert.DoesNotThrow(() => engine.Resolve<IConfigurationLoader>(), "IConfigurationLoader");
            Assert.DoesNotThrow(() => engine.Resolve<IStaticContentTransformer>(), "IStaticContentTransformer");
            Assert.DoesNotThrow(() => engine.Resolve<IDynamicContentTransformer>(), "IDynamicContentTransformer");
            Assert.DoesNotThrow(() => engine.Resolve<IIncludesHandler>(), "IIncludesHandler");
            Assert.DoesNotThrow(() => engine.Resolve<ILayoutParser>(), "ILayoutParser");
            Assert.DoesNotThrow(() => engine.Resolve<ILibraryLoader>(), "ILibraryLoader");
            Assert.DoesNotThrow(() => engine.Resolve<IProjectLoader>(), "IProjectLoader");
            Assert.DoesNotThrow(() => engine.Resolve<ILogger>(), "ILogger");
            Assert.DoesNotThrow(() => engine.Resolve<IPluginsManager>(), "IPluginsManager");
            Assert.DoesNotThrow(() => engine.Resolve<IPublisher>(), "IPublisher");
        }
    }
}
