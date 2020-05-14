//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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

namespace Core.Tests
{
    public static class LocalFileSystemConfigurationLoaderExtension 
    {
        public static Task<IConfiguration> Load(this LocalFileSystemConfigurationLoader loader, ILocation location) 
        {
            return loader.Load(new ILocation[] { location });
        }
    }

    public class LocalFileSystemConfigurationLoaderTest
    {
        [Test]
        public async Task Load_NoConfig() 
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(0, conf.Count);
        }

        [Test]
        public async Task Load_ComponentsLocations() 
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("components_dir: D:\\components\r\ncomponents:\r\n  - component1\r\n  - component2"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(0, conf.Count);
            Assert.AreEqual("D:\\components", conf.ComponentsFolder.ToPath());
            Assert.AreEqual(2, conf.Components.Count);
            Assert.Contains("component1", conf.Components);
            Assert.Contains("component2", conf.Components);
            Assert.AreEqual(0, conf.ThemesHierarchy.Count);
        }

        [Test]
        public async Task Load_PluginsLocations()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("plugins_dir: D:\\plugins\r\rplugins:\r\n  - plugin1\r\n  - plugin2"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(0, conf.Count);
            Assert.AreEqual("D:\\plugins", conf.PluginsFolder.ToPath());
            Assert.AreEqual(2, conf.Plugins.Count);
            Assert.Contains("plugin1", conf.Plugins);
            Assert.Contains("plugin2", conf.Plugins);
        }

        [Test]
        public async Task Load_WorkDirAndThemeLocations()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("a1: val\r\nwork_dir: D:\\work\r\nthemes_dir: D:\\themes\r\ntheme: theme1"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Xarial.Docify.Base.Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(1, conf.Count);
            Assert.That(conf.ContainsKey("a1"));
            Assert.AreEqual("D:\\work", conf.WorkingFolder);
            Assert.AreEqual("D:\\themes", conf.ThemesFolder.ToPath());
            Assert.AreEqual(1, conf.ThemesHierarchy.Count);
            Assert.AreEqual("theme1", conf.ThemesHierarchy[0]);
        }

        [Test]
        public async Task Load_DefaultThemesWorkingComponentsFolder()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            
            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.That(!string.IsNullOrEmpty(conf.WorkingFolder));
            Assert.That(!conf.ThemesFolder.IsEmpty());
            Assert.That(!conf.ComponentsFolder.IsEmpty());

            Assert.That(System.IO.Path.IsPathRooted(conf.WorkingFolder));
            Assert.That(System.IO.Path.IsPathRooted(conf.ThemesFolder.ToPath()));
            Assert.That(System.IO.Path.IsPathRooted(conf.ComponentsFolder.ToPath()));
        }

        [Test]
        public async Task Load_Config()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("a1: A\r\na2: B"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(2, conf.Count);
            Assert.AreEqual("A", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
        }

        [Test]
        public async Task Load_EnvSpecificConfTest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task Load_MultipleLocationsConflilctTest()
        {
            throw new NotImplementedException();
        }

        [Test]
        public async Task Load_ConfigWithTheme()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\themes\\theme1\\_config.yml", new MockFileData("x1: C\r\nx2: D\r\na1: E"));
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("theme: theme1\r\na1: A\r\na2: B\r\nthemes_dir: C:\\themes"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(4, conf.Count);
            Assert.AreEqual("A", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
            Assert.AreEqual("C", conf["x1"]);
            Assert.AreEqual("D", conf["x2"]);

            Assert.AreEqual(1, conf.ThemesHierarchy.Count);
            Assert.AreEqual("theme1", conf.ThemesHierarchy[0]);
            Assert.AreEqual("C:\\themes", conf.ThemesFolder.ToPath());
        }

        [Test]
        public async Task Load_ConfigWithNestedTheme() 
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\themes\\theme1\\_config.yml", new MockFileData("a: 1\r\nb: 2\r\nd: 6"));
            fs.AddFile("C:\\themes\\theme2\\_config.yml", new MockFileData("a: 3\r\nc: 4\r\ntheme: theme1"));
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("theme: theme2\r\nb: 5\r\nthemes_dir: C:\\themes"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

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
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\themes\\theme1\\_config.yml", new MockFileData("a:\r\n  - 1\r\n  - 2"));
            fs.AddFile("C:\\themes\\theme2\\_config.yml", new MockFileData("a:\r\n  - $\r\n  - 3\r\n  - 4\r\ntheme: theme1"));
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("theme: theme2\r\nb: 5\r\nthemes_dir: C:\\themes"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));
            
            Assert.AreEqual(2, conf.Count);
            Assert.AreEqual("5", conf["b"]);
            Assert.That((conf["a"] as IEnumerable<object>).SequenceEqual(new string[] { "1", "2", "3", "4" }));
        }
    }
}
