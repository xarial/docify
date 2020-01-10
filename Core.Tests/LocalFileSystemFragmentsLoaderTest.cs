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
    public class LocalFileSystemFragmentsLoaderTest
    {
        private ILoader m_Loader;

        [SetUp]
        public void Setup() 
        {
            var loaderMock = new Mock<ILoader>();

            loaderMock.Setup(m => m.Load(It.IsAny<Location>()))
                .Returns<Location>(l => Task.FromResult<IEnumerable<ISourceFile>>(new ISourceFile[] 
                {
                    new TextSourceFile(Location.FromPath("file1.txt"), "theme_f1"),
                    new TextSourceFile(Location.FromPath("dir\\file2.txt"), "")
                }));

            m_Loader = loaderMock.Object;
        }

        [Test]
        public async Task Load_Fragments() 
        {
            var frgLoader = new LocalFileSystemFragmentsLoader(m_Loader, new Configuration()
            {
                Fragments = new string[] {"A"}.ToList(),
                FragmentsFolder = Location.FromPath("C:\\fragments")
            });

            var res = await frgLoader.Load(new ISourceFile[] 
            {
                new TextSourceFile(Location.FromPath("file2.txt"), ""),
                new TextSourceFile(Location.FromPath("dir\\file3.txt"), "")
            });

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir-file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir-file3.txt"));
        }

        [Test]
        public async Task Load_Theme()
        {
            var frgLoader = new LocalFileSystemFragmentsLoader(m_Loader, new Configuration()
            {
                Theme = "A",
                ThemesFolder = Location.FromPath("C:\\themes")
            });

            var res = await frgLoader.Load(new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath("file1.txt"), "f1"),
                new TextSourceFile(Location.FromPath("file2.txt"), ""),
                new TextSourceFile(Location.FromPath("dir\\file3.txt"), "")
            });

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file1.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir-file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "file2.txt"));
            Assert.IsNotNull(res.First(f => f.Location.ToId() == "dir-file3.txt"));
            Assert.AreEqual("f1", (res.First(f => f.Location.ToId() == "file1.txt") as TextSourceFile).Content);
        }

        [Test]
        public void Load_Duplicate()
        {
            var frgLoader = new LocalFileSystemFragmentsLoader(m_Loader, new Configuration()
            {
                Fragments = new string[] { "A" }.ToList(),
                FragmentsFolder = Location.FromPath("C:\\fragments")
            });

            Assert.ThrowsAsync<DuplicateFragmentSourceFileException>(() => frgLoader.Load(new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath("dir\\file2.txt"), "")
            }));
        }
    }
}
