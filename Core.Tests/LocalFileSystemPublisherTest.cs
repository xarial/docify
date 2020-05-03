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
using Xarial.Docify.Core.Data;

namespace Core.Tests
{
    public class LocalFileSystemPublisherTest
    {
        [Test]
        public async Task WriteTextTest() 
        {
            var fs = new MockFileSystem();
            var publisher = new LocalFileSystemPublisher(new LocalFileSystemPublisherConfig(), fs);

            var pages = new Writable[]
            {
                new Writable("abc", Location.FromPath("page1.html")),
                new Writable("def", Location.FromPath("dir1\\page2.html")),
                new Writable("xyz", Location.FromPath("C:\\external\\page3.html")),
            };

            await publisher.Write(Location.FromPath("C:\\site"), pages);

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
            var publisher = new LocalFileSystemPublisher(new LocalFileSystemPublisherConfig(), fs);

            var assets = new IFile[]
            {
                new Writable(new byte[] { 1,2,3 }, Location.FromPath("file.bin"))
            };

            await publisher.Write(Location.FromPath("C:\\site"), assets);

            Assert.AreEqual(1, fs.Directory.GetFiles("C:\\site", "*.*", System.IO.SearchOption.AllDirectories).Length);
            Assert.IsTrue(fs.File.Exists("C:\\site\\file.bin"));
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(await fs.File.ReadAllBytesAsync("C:\\site\\file.bin")));
        }

        [Test]
        public async Task Write_ClearFolder()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page1.html", new MockFileData("xyz"));
            fs.AddFile("C:\\site\\page2.html", new MockFileData("klm"));

            var publisher = new LocalFileSystemPublisher(new LocalFileSystemPublisherConfig(), fs);

            var pages = new IFile[]
            {
                new Writable("abc", Location.FromPath("page1.html"))
            };

            await publisher.Write(Location.FromPath("C:\\site"), pages);

            Assert.AreEqual(1, fs.Directory.GetFiles("C:\\site", "*.*", System.IO.SearchOption.AllDirectories).Length);
            Assert.IsTrue(fs.File.Exists("C:\\site\\page1.html"));
            Assert.AreEqual("abc", await fs.File.ReadAllTextAsync("C:\\site\\page1.html"));
        }
    }
}
