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
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Base.Services;

namespace Core.Tests
{
    public class LocalFileSystemPublisherTest
    {
        [Test]
        public async Task WriteTextTest() 
        {
            var fs = new MockFileSystem();

            var publisher = new LocalFileSystemPublisher(fs, new PublisherExtension(),
                new Mock<ITargetDirectoryCleaner>().Object);
            
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

            var publisher = new LocalFileSystemPublisher(fs, new PublisherExtension(),
                new Mock<ITargetDirectoryCleaner>().Object);

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
            ILocation clearLoc = null;

            var targDirCleanerMock = new Mock<ITargetDirectoryCleaner>();
            targDirCleanerMock.Setup(m => m.ClearDirectory(It.IsAny<ILocation>()))
                .Returns((ILocation loc) => 
                {
                    clearLoc = loc;
                    return Task.CompletedTask;
                });

            var publisher = new LocalFileSystemPublisher(
                new MockFileSystem(), new PublisherExtension(),
                targDirCleanerMock.Object);

            var pages = new IFile[]
            {
                new FileMock(Location.FromPath("page1.html"), "abc")
            };

            await publisher.Write(Location.FromPath("C:\\site"), pages.ToAsyncEnumerable());

            Assert.AreEqual("C:\\site", clearLoc.ToPath());
        }


        [Test]
        public void ExistingFileOverrideTest()
        {
            var fs = new MockFileSystem();
            fs.AddFile("C:\\site\\page1.html", new MockFileData("xyz"));

            var publisher = new LocalFileSystemPublisher(fs, new PublisherExtension(),
                new Mock<ITargetDirectoryCleaner>().Object);

            var files = new IFile[]
            {
                new FileMock(Location.FromPath("C:\\site\\page1.html"), "abc")
            };

            Assert.ThrowsAsync<FilePublishOverwriteForbiddenException>(() => publisher.Write(Location.FromPath("C:\\site"), files.ToAsyncEnumerable()));
        }
    }
}
