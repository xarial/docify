//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Base.Data;
using System.Linq;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Plugin.Extensions;
using Tests.Common;

namespace Core.Tests
{
    public class ProjectLoaderTest
    {
        [Test]
        public async Task Load_MultipleLocationsTest()
        {
            var files = new Dictionary<string, IFile[]>();
            files.Add("C:\\site", new IFile[]
                {
                    new FileMock("index.md", "abc") ,
                    new FileMock("1.txt", "") 
                });

            files.Add("C:\\site1", new IFile[]
                {
                    new FileMock(@"page2\index.md", "xyz"),
                    new FileMock(@"page2\2.txt", "")
                });

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[]  f) => files[l.ToPath()].ToAsyncEnumerable());

            var loader = new ProjectLoader(fileLoaderMock.Object,
                new Mock<ILibraryLoader>().Object,
                new Mock<IPluginsManager>().Object,
                new Configuration(), 
                new Mock<ILoaderExtension>().Object,
                new Mock<ILogger>().Object);
            
            var res = await loader.Load(new ILocation[] 
            {
                Location.FromPath("C:\\site"), 
                Location.FromPath("C:\\site1") 
            }).ToListAsync();

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "index.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "1.txt"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page2::index.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page2::2.txt"));
        }

        [Test]
        public async Task Load_MultipleLocationsConflictTest()
        {
            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock(@"index.md", ""),
                }.ToAsyncEnumerable());

            var loader = new ProjectLoader(fileLoaderMock.Object,
                new Mock<ILibraryLoader>().Object,
                new Mock<IPluginsManager>().Object,
                new Configuration(),
                new Mock<ILoaderExtension>().Object,
                new Mock<ILogger>().Object);

            await AssertException.ThrowsOfTypeAsync<DuplicateFileException>(async () =>
            {
                await loader.Load(new ILocation[] { Location.FromPath("C:\\site"), Location.FromPath("C:\\site1") }).ToListAsync();
            });
        }

        [Test]
        public async Task Load_ExcludePluginFilesTest()
        {
            var fs = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
            fs.AddFile("C:\\site\\index.md", new System.IO.Abstractions.TestingHelpers.MockFileData(""));
            fs.AddFile("C:\\site\\_plugins\\abc\\plugin.dll", new System.IO.Abstractions.TestingHelpers.MockFileData(""));
            
            var loader = new ProjectLoader(
                new LocalFileSystemFileLoader(fs, new Mock<ILogger>().Object),
                new Mock<ILibraryLoader>().Object,
                new Mock<IPluginsManager>().Object,
                new Configuration(),
                new Mock<ILoaderExtension>().Object,
                new Mock<ILogger>().Object);

            var res = await loader.Load(new ILocation[]
            {
                Location.FromPath("C:\\site"),
            }).ToListAsync();

            Assert.AreEqual(1, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "index.md"));
        }

        [Test]
        public async Task LoadLibrary_Components()
        {
            var libLoaderMock = new Mock<ILibraryLoader>();

            libLoaderMock.Setup(m => m.LoadComponentFiles(It.IsAny<string>(), It.IsAny<string[]>())).Returns(
                (string n, string[] f) => 
                {
                    if (n == "A" && f?.Any() != true)
                    {
                        return new IFile[]
                        {
                            new FileMock(Location.FromPath("file2.txt"), "a"),
                            new FileMock(Location.FromPath("dir\\file3.txt"), "b")
                        }.ToAsyncEnumerable();
                    }
                    else
                    {
                        throw new Exception();
                    }
                });

            var loader = new ProjectLoader(new Mock<IFileLoader>().Object,
                libLoaderMock.Object,
                new Mock<IPluginsManager>().Object,
                new Configuration() { Components = new string[] { "A" }.ToList() },
                new Mock<ILoaderExtension>().Object,
                new Mock<ILogger>().Object);

            var res = await loader.Load(new ILocation[0]).ToListAsync();

            Assert.AreEqual(2, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file3.txt"));
            Assert.AreEqual("a", res.First(f => f.Location.ToId() == "file2.txt").AsTextContent());
            Assert.AreEqual("b", res.First(f => f.Location.ToId() == "dir::file3.txt").AsTextContent());
        }

        [Test]
        public async Task LoadLibrary_Theme()
        {
            var libLoaderMock = new Mock<ILibraryLoader>();
            libLoaderMock.Setup(m => m.LoadThemeFiles(It.IsAny<string>(), It.IsAny<string[]>())).Returns(
                (string n, string[] f) =>
                {
                    if (n == "A" && f?.Any() != true)
                    {
                        return new IFile[]
                        {
                            new FileMock(Location.FromPath("file1.txt"), $"{n}_theme_f1"),
                            new FileMock(Location.FromPath("dir\\file2.txt"), $"{n}_theme_f2")
                        }.ToAsyncEnumerable();
                    }
                    else
                    {
                        throw new Exception();
                    }
                });

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock(Location.FromPath("file1.txt"), "f1"),
                    new FileMock(Location.FromPath("file2.txt"), ""),
                    new FileMock(Location.FromPath("dir\\file3.txt"), "")
                }.ToAsyncEnumerable());

            var conf = new Configuration();

            conf.ThemesHierarchy.Add("A");

            var loader = new ProjectLoader(fileLoaderMock.Object,
                libLoaderMock.Object, new Mock<IPluginsManager>().Object, conf, 
                new Mock<ILoaderExtension>().Object, 
                new Mock<ILogger>().Object);
            
            var res = await loader.Load(new ILocation[] { Location.FromPath("") }).ToListAsync();

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file3.txt"));
            Assert.AreEqual("f1", res.First(f => f.Location.ToId() == "file1.txt").AsTextContent());
            Assert.AreEqual("A_theme_f2", res.First(f => f.Location.ToId() == "dir::file2.txt").AsTextContent());
        }

        [Test]
        public async Task LoadLibrary_ThemeNested()
        {
            var libLoader = new Mock<ILibraryLoader>();
            libLoader.Setup(m => m.LoadThemeFiles(It.IsAny<string>(), It.IsAny<string[]>())).Returns(
                (string n, string[] f) =>
                {
                    if (n == "A")
                    {
                        return new IFile[]
                        {
                            new FileMock(Location.FromPath("dir\\file2.txt"), $"{n}_theme_f2"),
                            new FileMock(Location.FromPath("dir\\file3.txt"), $"{n}_theme_f3"),
                            new FileMock(Location.FromPath("file4.txt"), $"{n}_theme_f4"),
                            new FileMock(Location.FromPath("dir\\file4.txt"), $"{n}_theme_dir-f4")
                        }.ToAsyncEnumerable();
                    }
                    else if (n == "B")
                    {
                        return new IFile[]
                        {
                            new FileMock(Location.FromPath("file1.txt"), $"{n}_theme_f1"),
                            new FileMock(Location.FromPath("dir\\file2.txt"), $"{n}_theme_f2"),
                            new FileMock(Location.FromPath("dir\\file4.txt"), $"{n}_theme_f4")
                        }.ToAsyncEnumerable();
                    }                    
                    else
                    {
                        throw new Exception();
                    }
                });

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock(Location.FromPath("dir\\file2.txt"), "f2"),
                    new FileMock(Location.FromPath("dir\\file3.txt"), "f3"),
                    new FileMock(Location.FromPath("file5.txt"), "f5")
                }.ToAsyncEnumerable());

            var conf = new Configuration();

            conf.ThemesHierarchy.Add("A");
            conf.ThemesHierarchy.Add("B");

            var loader = new ProjectLoader(fileLoaderMock.Object,
                libLoader.Object, new Mock<IPluginsManager>().Object, conf, new Mock<ILoaderExtension>().Object,
                new Mock<ILogger>().Object);

            var res = await loader.Load(new ILocation[] { Location.FromPath("") }).ToListAsync();
            
            Assert.AreEqual(6, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file3.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file4.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file5.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file4.txt"));

            Assert.AreEqual("B_theme_f1", res.First(f => f.Location.ToId() == "file1.txt").AsTextContent());
            Assert.AreEqual("f2", res.First(f => f.Location.ToId() == "dir::file2.txt").AsTextContent());
            Assert.AreEqual("f3", res.First(f => f.Location.ToId() == "dir::file3.txt").AsTextContent());
            Assert.AreEqual("A_theme_f4", res.First(f => f.Location.ToId() == "file4.txt").AsTextContent());
            Assert.AreEqual("f5", res.First(f => f.Location.ToId() == "file5.txt").AsTextContent());
            Assert.AreEqual("A_theme_dir-f4", res.First(f => f.Location.ToId() == "dir::file4.txt").AsTextContent());
        }

        [Test]
        public void LoadLibrary_DuplicateComponents()
        {
            var libLoader = new Mock<ILibraryLoader>();
            libLoader.Setup(m => m.LoadComponentFiles(It.IsAny<string>(), It.IsAny<string[]>())).Returns(
                (string n, string[] f) => new IFile[]
                {
                    new FileMock(Location.FromPath("file1.txt"), "")
                }.ToAsyncEnumerable());

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) => new IFile[]
                {
                    new FileMock(Location.FromPath("file1.txt"), "")
                }.ToAsyncEnumerable());

            var conf = new Configuration()
            {
                Components = new string[] { "A" }.ToList(),
            };

            var loader = new ProjectLoader(fileLoaderMock.Object,
                libLoader.Object, new Mock<IPluginsManager>().Object, conf, new Mock<ILoaderExtension>().Object, new Mock<ILogger>().Object);

            Assert.Throws<DuplicateComponentSourceFileException>(
                () => loader.Load(new ILocation[] { Location.FromPath("") }).ToEnumerable().ToList());
        }
    }
}
