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
using System.Threading.Tasks;
using Xarial.Docify.Core.Loader;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Publisher;
using Xarial.Docify.Base;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core;
using Tests.Common.Mocks;
using Xarial.Docify.Core.Plugin.Extensions;
using Moq;
using Xarial.Docify.Base.Plugins;

namespace Core.Tests
{
    public class LocalFileSystemPublisherTest
    {
        private LocalFileSystemPublisher NewPublisher(MockFileSystem fs) 
        {
            var pubExt = new Mock<IPublisherExtension>();
            pubExt.Setup(m => m.PrePublishFile(It.IsAny<ILocation>(), It.IsAny<IFile>()))
                .Returns((ILocation c, IFile f) => Task.FromResult(new PrePublishResult() { File = f, SkipFile = false }));

            return new LocalFileSystemPublisher(fs, pubExt.Object);
        }

        [Test]
        public async Task WriteTextTest() 
        {
            var fs = new MockFileSystem();
            var publisher = NewPublisher(fs);

            var pages = new FileMock[]
            {
                new FileMock(Location.FromPath("page1.html"), "abc"),
                new FileMock(Location.FromPath("dir1\\page2.html"), "def"),
                new FileMock(Location.FromPath("C:\\external\\page3.html"), "xyz"),
            };

            await publisher.Write(Location.FromPath("C:\\site"), pages.ToAsyncEnumerable());

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
            var publisher = NewPublisher(fs);

            var assets = new IFile[]
            {
                new FileMock(Location.FromPath("file.bin"), new byte[] { 1,2,3 })
            };

            await publisher.Write(Location.FromPath("C:\\site"), assets.ToAsyncEnumerable());

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

            var publisher = NewPublisher(fs);

            var pages = new IFile[]
            {
                new FileMock(Location.FromPath("page1.html"), "abc")
            };

            await publisher.Write(Location.FromPath("C:\\site"), pages.ToAsyncEnumerable());

            Assert.AreEqual(1, fs.Directory.GetFiles("C:\\site", "*.*", System.IO.SearchOption.AllDirectories).Length);
            Assert.IsTrue(fs.File.Exists("C:\\site\\page1.html"));
            Assert.AreEqual("abc", await fs.File.ReadAllTextAsync("C:\\site\\page1.html"));
        }
    }
}
