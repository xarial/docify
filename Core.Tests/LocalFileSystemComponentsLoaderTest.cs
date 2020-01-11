//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Loader;

namespace Core.Tests
{
    public class LocalFileSystemComponentsLoaderTest
    {
        private ILoader m_Loader;

        [SetUp]
        public void Setup() 
        {
            var loaderMock = new Mock<ILoader>();

            loaderMock.Setup(m => m.Load(It.IsAny<Location>()))
                .Returns<Location>(l => Task.FromResult<IEnumerable<ISourceFile>>(new ISourceFile[] 
                {
                    new TextSourceFile(Location.FromPath("file1.txt"), $"{l.Path.Last()}_theme_f1"),
                    new TextSourceFile(Location.FromPath("dir\\file2.txt"), $"{l.Path.Last()}_theme_f2")
                }));

            m_Loader = loaderMock.Object;
        }

        [Test]
        public async Task Load_Components() 
        {
            var frgLoader = new LocalFileSystemComponentsLoader(m_Loader, new Configuration()
            {
                Components = new string[] { "A" }.ToList(),
                ComponentsFolder = Location.FromPath("C:\\components")
            });

            var res = await frgLoader.Load(new ISourceFile[] 
            {
                new TextSourceFile(Location.FromPath("file2.txt"), ""),
                new TextSourceFile(Location.FromPath("dir\\file3.txt"), "")
            });

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file3.txt"));
        }

        [Test]
        public async Task Load_Theme()
        {
            var conf = new Configuration()
            {
                ThemesFolder = Location.FromPath("C:\\themes")
            };

            conf.ThemesHierarchy.Add("A");

            var frgLoader = new LocalFileSystemComponentsLoader(m_Loader, conf);

            var res = await frgLoader.Load(new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath("file1.txt"), "f1"),
                new TextSourceFile(Location.FromPath("file2.txt"), ""),
                new TextSourceFile(Location.FromPath("dir\\file3.txt"), "")
            });

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file3.txt"));
            Assert.AreEqual("f1", (res.First(f => f.Location.ToId() == "file1.txt") as TextSourceFile).Content);
            Assert.AreEqual("A_theme_f2", (res.First(f => f.Location.ToId() == "dir::file2.txt") as TextSourceFile).Content);
        }

        [Test]
        public async Task Load_ThemeNested() 
        {
            var loaderMock = new Mock<ILoader>();

            loaderMock.Setup(m => m.Load(It.IsAny<Location>()))
                .Returns<Location>(l =>
                {
                    ISourceFile[] res = null;
                    if (l.Path.Last() == "A")
                    {
                        res = new ISourceFile[]
                        {
                            new TextSourceFile(Location.FromPath("dir\\file2.txt"), $"{l.Path.Last()}_theme_f2"),
                            new TextSourceFile(Location.FromPath("dir\\file3.txt"), $"{l.Path.Last()}_theme_f3"),
                            new TextSourceFile(Location.FromPath("file4.txt"), $"{l.Path.Last()}_theme_f4"),
                            new TextSourceFile(Location.FromPath("dir\\file4.txt"), $"{l.Path.Last()}_theme_dir-f4")
                        };
                    }
                    else if (l.Path.Last() == "B")
                    {
                        res = new ISourceFile[]
                        {
                            new TextSourceFile(Location.FromPath("file1.txt"), $"{l.Path.Last()}_theme_f1"),
                            new TextSourceFile(Location.FromPath("dir\\file2.txt"), $"{l.Path.Last()}_theme_f2"),
                            new TextSourceFile(Location.FromPath("dir\\file4.txt"), $"{l.Path.Last()}_theme_f4")
                        };
                    }

                    return Task.FromResult<IEnumerable<ISourceFile>>(res);
                });

            var conf = new Configuration()
            {
                ThemesFolder = Location.FromPath("C:\\themes")
            };

            conf.ThemesHierarchy.Add("A");
            conf.ThemesHierarchy.Add("B");

            var frgLoader = new LocalFileSystemComponentsLoader(loaderMock.Object, conf);

            var res = await frgLoader.Load(new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath("dir\\file2.txt"), "f2"),
                new TextSourceFile(Location.FromPath("dir\\file3.txt"), "f3"),
                new TextSourceFile(Location.FromPath("file5.txt"), "f5")
            });

            Assert.AreEqual(6, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file3.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file4.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file5.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir::file4.txt"));

            Assert.AreEqual("B_theme_f1", (res.First(f => f.Location.ToId() == "file1.txt") as TextSourceFile).Content);
            Assert.AreEqual("f2", (res.First(f => f.Location.ToId() == "dir::file2.txt") as TextSourceFile).Content);
            Assert.AreEqual("f3", (res.First(f => f.Location.ToId() == "dir::file3.txt") as TextSourceFile).Content);
            Assert.AreEqual("A_theme_f4", (res.First(f => f.Location.ToId() == "file4.txt") as TextSourceFile).Content);
            Assert.AreEqual("f5", (res.First(f => f.Location.ToId() == "file5.txt") as TextSourceFile).Content);
            Assert.AreEqual("A_theme_dir-f4", (res.First(f => f.Location.ToId() == "dir::file4.txt") as TextSourceFile).Content);
        }

        [Test]
        public void Load_Duplicate()
        {
            var compLoader = new LocalFileSystemComponentsLoader(m_Loader, new Configuration()
            {
                Components = new string[] { "A" }.ToList(),
                ComponentsFolder = Location.FromPath("C:\\components")
            });

            Assert.ThrowsAsync<DuplicateComponentSourceFileException>(() => compLoader.Load(new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath("dir\\file2.txt"), "")
            }));
        }
    }
}
