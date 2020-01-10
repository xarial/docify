//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Core.Loader;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Publisher;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;

namespace Core.Tests
{
    public class LocalFileSystemPublisherTest
    {
        [Test]
        public async Task WriteTextTest() 
        {
            var fs = new MockFileSystem();
            var publisher = new LocalFileSystemPublisher(new LocalFileSystemPublisherConfig("C:\\site"), fs);

            var pages = new Page[]
            {
                new Page(Location.FromPath("page1.html"), "") { Content  = "abc" },
                new Page(Location.FromPath("dir1\\page2.html"), "") { Content  = "def" },
                new Page(Location.FromPath("C:\\external\\page3.html"), "") { Content  = "xyz" },
            };

            await publisher.Write(pages);

            Assert.AreEqual(2, fs.Directory.GetFiles("C:\\site", "*.*", System.IO.SearchOption.AllDirectories).Length);
            Assert.IsTrue(fs.File.Exists("C:\\site\\page1.html"));
            Assert.IsTrue(fs.File.Exists("C:\\site\\dir1\\page2.html"));
            Assert.IsTrue(fs.File.Exists("C:\\external\\page3.html"));
            Assert.AreEqual("abc", await fs.File.ReadAllTextAsync("C:\\site\\page1.html"));
            Assert.AreEqual("def", await fs.File.ReadAllTextAsync("C:\\site\\dir1\\page2.html"));
            Assert.AreEqual("xyz", await fs.File.ReadAllTextAsync("C:\\external\\page3.html"));
        }

        [Test]
        public async Task WriteBinaryTest()
        {
            var fs = new MockFileSystem();
            var publisher = new LocalFileSystemPublisher(new LocalFileSystemPublisherConfig("C:\\site"), fs);

            var assets = new BinaryAsset[]
            {
                new BinaryAsset(new byte[] { 1,2,3 }, Location.FromPath("file.bin"))
            };

            await publisher.Write(assets);

            Assert.AreEqual(1, fs.Directory.GetFiles("C:\\site", "*.*", System.IO.SearchOption.AllDirectories).Length);
            Assert.IsTrue(fs.File.Exists("C:\\site\\file.bin"));
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(await fs.File.ReadAllBytesAsync("C:\\site\\file.bin")));
        }
    }
}
