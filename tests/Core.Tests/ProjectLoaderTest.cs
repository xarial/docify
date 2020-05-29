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
                new Configuration());
            
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
                new Configuration());

            Exception ex = null;

            try
            {
                await loader.Load(new ILocation[] { Location.FromPath("C:\\site"), Location.FromPath("C:\\site1") }).ToListAsync();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsInstanceOf<DuplicateFileException>(ex);
        }
    }
}
