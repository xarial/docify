using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;

namespace CLI.Tests
{
    public class DependencyTest
    {
        [Test]
        public void ResolveTest() 
        {
            var engine = new DocifyEngineMock(@"D:\src", @"D:\out", "www.xarial.com", Environment_e.Test);

            Assert.DoesNotThrow(() => engine.Resove<ICompiler>(), "ICompiler");
            Assert.DoesNotThrow(() => engine.Resove<IComponentsLoader>(), "IComponentsLoader");
            Assert.DoesNotThrow(() => engine.Resove<IComposer>(), "IComposer");
            Assert.DoesNotThrow(() => engine.Resove<IConfigurationLoader>(), "IConfigurationLoader");
            Assert.DoesNotThrow(() => engine.Resove<IContentTransformer>(), "IContentTransformer");
            Assert.DoesNotThrow(() => engine.Resove<IIncludesHandler>(), "IIncludesHandler");
            Assert.DoesNotThrow(() => engine.Resove<ILayoutParser>(), "ILayoutParser");
            Assert.DoesNotThrow(() => engine.Resove<ILoader>(), "ILoader");
            Assert.DoesNotThrow(() => engine.Resove<ILogger>(), "ILogger");
            Assert.DoesNotThrow(() => engine.Resove<IPluginsManager>(), "IPluginsManager");
            Assert.DoesNotThrow(() => engine.Resove<IPublisher>(), "IPublisher");
        }
    }
}
