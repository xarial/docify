//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Loader;
using System.Linq;
using Moq;
using Xarial.Docify.Base.Services;
using Tests.Common.Mocks;

namespace Core.Tests
{   public class ConfigurationLoaderTest
    {
        [Test]
        public async Task Load_NoConfig() 
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => AsyncEnumerable.Empty<IFile>());
            
            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(0, conf.Count);
        }

        [Test]
        public async Task Load_ComponentNames() 
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] { new FileMock("_config.yml", "components:\r\n  - component1\r\n  - component2") }.ToAsyncEnumerable());

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(0, conf.Count);
            Assert.AreEqual(2, conf.Components.Count);
            Assert.Contains("component1", conf.Components);
            Assert.Contains("component2", conf.Components);
            Assert.AreEqual(0, conf.ThemesHierarchy.Count);
        }

        [Test]
        public async Task Load_EmptyConfig()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] { new FileMock("_config.yml", "") }.ToAsyncEnumerable());

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(0, conf.Count);
        }

        [Test]
        public async Task Load_NoEnvironment()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] { new FileMock("_config.yml", "a: b") }.ToAsyncEnumerable());

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, null);

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(1, conf.Count);
            Assert.AreEqual("b", conf["a"]);
        }

        [Test]
        public async Task Load_PluginNames()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] { new FileMock("_config.yml", "plugins:\r\n  - plugin1\r\n  - plugin2") }.ToAsyncEnumerable());
            
            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(0, conf.Count);
            Assert.AreEqual(2, conf.Plugins.Count);
            Assert.Contains("plugin1", conf.Plugins);
            Assert.Contains("plugin2", conf.Plugins);
        }

        [Test]
        public async Task Load_ThemeName()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] { new FileMock("_config.yml", "a1: val\r\ntheme: theme1") }.ToAsyncEnumerable());

            var libLoaderMock = new Mock<ILibraryLoader>();
            libLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation t, string[] f) => AsyncEnumerable.Empty<IFile>());

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, libLoaderMock.Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(1, conf.Count);
            Assert.That(conf.ContainsKey("a1"));
            Assert.AreEqual(1, conf.ThemesHierarchy.Count);
            Assert.AreEqual("theme1", conf.ThemesHierarchy[0]);
        }
        
        [Test]
        public async Task Load_Config()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] { new FileMock("_config.yml", "a1: A\r\na2: B") }.ToAsyncEnumerable());
            
            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(2, conf.Count);
            Assert.AreEqual("A", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
        }

        [Test]
        public async Task Load_FiltersTest()
        {
            string[] filters = null;
            string[] themeFilters = null;
            string loc = "";
            ILocation theme = Location.Empty;

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l.ToPath();
                    filters = f;
                    return new IFile[]
                    {
                        new FileMock("_config.yml", "theme: theme1")
                    }.ToAsyncEnumerable();
                });

            var libLoaderMock = new Mock<ILibraryLoader>();
            libLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation t, string[] f)=>
                {
                    theme = t;
                    themeFilters = f;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, libLoaderMock.Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(2, filters.Length);
            Assert.Contains("_config.yml", filters);
            Assert.Contains("_config.Test.yml", filters);
            Assert.AreEqual(2, themeFilters.Length);
            Assert.Contains("_config.yml", themeFilters);
            Assert.Contains("_config.Test.yml", themeFilters);
            Assert.AreEqual("C:\\site", loc);
            Assert.AreEqual("_themes::theme1", theme.ToId());
        }

        [Test]
        public async Task Load_EnvSpecificConfTest()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[] 
                {
                    new FileMock("_config.yml", "a1: A\r\na2: B"),
                    new FileMock("_config.test.yml", "a1: A1\r\na3: C")
                }.ToAsyncEnumerable());

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(3, conf.Count);
            Assert.AreEqual("A1", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
            Assert.AreEqual("C", conf["a3"]);
        }

        [Test]
        public async Task Load_MultipleLocationsConfigsTest()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => 
                {
                    if (l.ToPath() == "C:\\site")
                    {
                        return new IFile[]
                        {
                            new FileMock("_config.yml", "a1: A\r\na2: B")
                        }.ToAsyncEnumerable();
                    }
                    else if (l.ToPath() == "C:\\site1") 
                    {
                        return new IFile[]
                        {
                            new FileMock("_config.yml", "a1: A1\r\na3: C")
                        }.ToAsyncEnumerable();
                    }
                    else
                    {
                        throw new Exception();
                    }
                });

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, new Mock<ILibraryLoader>().Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site"),
                Location.FromPath("C:\\site1") });

            Assert.AreEqual(3, conf.Count);
            Assert.AreEqual("A1", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
            Assert.AreEqual("C", conf["a3"]);
        }

        [Test]
        public async Task Load_ConfigWithTheme()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock("_config.yml", "theme: theme1\r\na1: A\r\na2: B")
                }.ToAsyncEnumerable());

            var libLoaderMock = new Mock<ILibraryLoader>();
            libLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation t, string[] f) => new IFile[]
                {
                    new FileMock("_config.yml", "x1: C\r\nx2: D\r\na1: E")
                }.ToAsyncEnumerable());

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, libLoaderMock.Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(4, conf.Count);
            Assert.AreEqual("A", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
            Assert.AreEqual("C", conf["x1"]);
            Assert.AreEqual("D", conf["x2"]);

            Assert.AreEqual(1, conf.ThemesHierarchy.Count);
            Assert.AreEqual("theme1", conf.ThemesHierarchy[0]);
        }

        [Test]
        public async Task Load_ConfigWithNestedTheme() 
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock("_config.yml", "theme: theme2\r\nb: 5")
                }.ToAsyncEnumerable());

            var libLoaderMock = new Mock<ILibraryLoader>();
            libLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation t, string[] f) =>
                {
                    if (t.Segments[0] == "_themes" && t.Segments[1] == "theme1")
                    {
                        return new IFile[]
                        {
                            new FileMock("_config.yml", "a: 1\r\nb: 2\r\nd: 6")
                        }.ToAsyncEnumerable();
                    }
                    if (t.Segments[0] == "_themes" && t.Segments[1] == "theme2")
                    {
                        return new IFile[]
                        {
                            new FileMock("_config.yml", "a: 3\r\nc: 4\r\ntheme: theme1")
                        }.ToAsyncEnumerable();
                    }
                    else
                    {
                        throw new Exception();
                    }
                });

            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, libLoaderMock.Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });

            Assert.AreEqual(2, conf.ThemesHierarchy.Count);
            Assert.AreEqual("theme2", conf.ThemesHierarchy[0]);
            Assert.AreEqual("theme1", conf.ThemesHierarchy[1]);

            Assert.AreEqual(4, conf.Count);
            Assert.AreEqual("3", conf["a"]);
            Assert.AreEqual("5", conf["b"]);
            Assert.AreEqual("4", conf["c"]);
            Assert.AreEqual("6", conf["d"]);
        }

        [Test]
        public async Task Load_ConfigWithNestedThemeInheritList()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock("_config.yml", "theme: theme2\r\nb: 5")
                }.ToAsyncEnumerable());

            var libLoaderMock = new Mock<ILibraryLoader>();
            libLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation t, string[] f) =>
                {
                    if (t.Segments[0] == "_themes" && t.Segments[1] == "theme1")
                    {
                        return new IFile[]
                        {
                            new FileMock("_config.yml", "a:\r\n  - 1\r\n  - 2")
                        }.ToAsyncEnumerable();
                    }
                    if (t.Segments[0] == "_themes" && t.Segments[1] == "theme2")
                    {
                        return new IFile[]
                        {
                            new FileMock("_config.yml", "a:\r\n  - $\r\n  - 3\r\n  - 4\r\ntheme: theme1")
                        }.ToAsyncEnumerable();
                    }
                    else
                    {
                        throw new Exception();
                    }
                });
            
            var confLoader = new ConfigurationLoader(
                fileLoaderMock.Object, libLoaderMock.Object, "Test");

            var conf = await confLoader.Load(new ILocation[] { Location.FromPath("C:\\site") });
            
            Assert.AreEqual(2, conf.Count);
            Assert.AreEqual("5", conf["b"]);
            Assert.That((conf["a"] as IEnumerable<object>).SequenceEqual(new string[] { "1", "2", "3", "4" }));
        }
    }
}
