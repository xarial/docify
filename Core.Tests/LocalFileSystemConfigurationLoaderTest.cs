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
using Xarial.Docify.Core;

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
    }
}
