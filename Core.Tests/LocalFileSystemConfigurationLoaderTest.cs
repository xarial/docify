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

            var confLoader = new LocalFileSystemConfigurationLoader("C:\\site", fs, Xarial.Docify.Base.Environment_e.Test);

            var conf = await confLoader.Load();

            Assert.AreEqual(0, conf.Count);
        }

        [Test]
        public async Task Load_Fragments() 
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("fragments_dir: D:\\fragments\r\nfragments:\r\n  - fragment1\r\n  - fragment2"));

            var confLoader = new LocalFileSystemConfigurationLoader("C:\\site", fs, Xarial.Docify.Base.Environment_e.Test);

            var conf = await confLoader.Load();

            Assert.AreEqual(0, conf.Count);
            Assert.AreEqual("D:\\fragments", conf.FragmentsFolder);
            Assert.AreEqual(2, conf.Fragments.Count);
            Assert.Contains("fragment1", conf.Fragments);
            Assert.Contains("fragment2", conf.Fragments);
        }

        [Test]
        public async Task Load_WorkDirAndTheme()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("a1: val\r\nwork_dir: D:\\work\r\nthemes_dir: D:\\themes\r\ntheme: theme1"));

            var confLoader = new LocalFileSystemConfigurationLoader("C:\\site", fs, Xarial.Docify.Base.Environment_e.Test);

            var conf = await confLoader.Load();

            Assert.AreEqual(1, conf.Count);
            Assert.That(conf.ContainsKey("a1"));
            Assert.AreEqual("D:\\work", conf.WorkingFolder);
            Assert.AreEqual("D:\\themes", conf.ThemesFolder);
            Assert.AreEqual("theme1", conf.Theme);
        }

        [Test]
        public async Task Load_DefaultThemesWorkingFragmentsFolder()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            
            var confLoader = new LocalFileSystemConfigurationLoader("C:\\site", fs, Xarial.Docify.Base.Environment_e.Test);

            var conf = await confLoader.Load();

            Assert.That(!string.IsNullOrEmpty(conf.WorkingFolder));
            Assert.That(!string.IsNullOrEmpty(conf.ThemesFolder));
            Assert.That(!string.IsNullOrEmpty(conf.FragmentsFolder));

            Assert.That(System.IO.Path.IsPathRooted(conf.WorkingFolder));
            Assert.That(System.IO.Path.IsPathRooted(conf.ThemesFolder));
            Assert.That(System.IO.Path.IsPathRooted(conf.FragmentsFolder));
        }

        [Test]
        public async Task Load_Config()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page.html", null);
            fs.AddFile("C:\\site\\_config.yml", new MockFileData("a1: A\r\na2: B"));

            var confLoader = new LocalFileSystemConfigurationLoader("C:\\site", fs, Xarial.Docify.Base.Environment_e.Test);

            var conf = await confLoader.Load();

            Assert.AreEqual(2, conf.Count);
            Assert.AreEqual("A", conf["a1"]);
            Assert.AreEqual("B", conf["a2"]);
        }

        [Test]
        public void LocalFileSystemLoaderConfig_IgnoreEmptyTest() 
        {
            var config = new LocalFileSystemLoaderConfig(@"D:\data", 
                new Configuration(Xarial.Docify.Base.Environment_e.Test));

            Assert.AreEqual(@"D:\data", config.Path);
            Assert.IsEmpty(config.Ignore);
        }

        [Test]
        public void LocalFileSystemLoaderConfig_IgnoreTest()
        {
            var config = new LocalFileSystemLoaderConfig(@"D:\data",
                new Configuration(new Dictionary<string, dynamic>() 
                {
                    { 
                        "ignore", new List<string>(new string[] { "A", "B" })
                    }
                }, Xarial.Docify.Base.Environment_e.Test));

            Assert.AreEqual(@"D:\data", config.Path);
            Assert.AreEqual(2, config.Ignore.Count);
            Assert.Contains("A", config.Ignore);
            Assert.Contains("B", config.Ignore);
        }

        [Test]
        public void LocalFileSystemLoaderConfig_IgnoreInvalidCastTest()
        {
            Assert.Throws<InvalidCastException>(() => new LocalFileSystemLoaderConfig(@"D:\data",
                    new Configuration(new Dictionary<string, dynamic>()
                    {
                        {
                            "ignore", "A"
                        }
                    }, Xarial.Docify.Base.Environment_e.Test)));
        }
    }
}
