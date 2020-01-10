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

namespace Core.Tests
{
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
        public async Task Load_FragmentsLocations() 
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("fragments_dir: D:\\fragments\r\nfragments:\r\n  - fragment1\r\n  - fragment2"));

            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.AreEqual(0, conf.Count);
            Assert.AreEqual("D:\\fragments", conf.FragmentsFolder.ToPath());
            Assert.AreEqual(2, conf.Fragments.Count);
            Assert.Contains("fragment1", conf.Fragments);
            Assert.Contains("fragment2", conf.Fragments);
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
            Assert.AreEqual("theme1", conf.Theme);
        }

        [Test]
        public async Task Load_DefaultThemesWorkingFragmentsFolder()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            
            var confLoader = new LocalFileSystemConfigurationLoader(fs, Environment_e.Test);

            var conf = await confLoader.Load(Location.FromPath("C:\\site"));

            Assert.That(!string.IsNullOrEmpty(conf.WorkingFolder));
            Assert.That(!conf.ThemesFolder.IsEmpty);
            Assert.That(!conf.FragmentsFolder.IsEmpty);

            Assert.That(System.IO.Path.IsPathRooted(conf.WorkingFolder));
            Assert.That(System.IO.Path.IsPathRooted(conf.ThemesFolder.ToPath()));
            Assert.That(System.IO.Path.IsPathRooted(conf.FragmentsFolder.ToPath()));
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

            Assert.AreEqual("theme1", conf.Theme);
            Assert.AreEqual("C:\\themes", conf.ThemesFolder.ToPath());
        }
    }
}
