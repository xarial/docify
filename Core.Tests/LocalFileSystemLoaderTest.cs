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
using Xarial.Docify.Base;

namespace Core.Tests
{
    public class LocalFileSystemLoaderTest
    {
        [Test]
        public async Task Load_TextAndBinaryTest() 
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(@"C:\site", new string[0]),
            new MockFileSystem(new Dictionary<string, MockFileData>() 
            {
                { @"C:\page2.md", null },
                { @"C:\site\page1.md", new MockFileData("abc") },
                { @"C:\site\page2.html", null },
                { @"C:\site\folder\1.txt", null },
                { @"C:\site\img\img1.png", new MockFileData(new byte[] { 1, 2, 3 }) },
                { @"C:\site\test1.xlsx", null },
            }
            ));

            var res = await loader.Load();

            Assert.AreEqual(5, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page1.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page2.html"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "folder-1.txt"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "img-img1.png"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "test1.xlsx"));
            Assert.IsInstanceOf<ITextSourceFile>(res.FirstOrDefault(f => f.Location.ToId() == "page1.md"));
            Assert.IsInstanceOf<ITextSourceFile>(res.FirstOrDefault(f => f.Location.ToId() == "page2.html"));
            Assert.IsInstanceOf<ITextSourceFile>(res.FirstOrDefault(f => f.Location.ToId() == "folder-1.txt"));
            Assert.IsInstanceOf<IBinarySourceFile>(res.FirstOrDefault(f => f.Location.ToId() == "img-img1.png"));
            Assert.IsInstanceOf<IBinarySourceFile>(res.FirstOrDefault(f => f.Location.ToId() == "test1.xlsx"));
            Assert.AreEqual("abc", (res.FirstOrDefault(f => f.Location.ToId() == "page1.md") as ITextSourceFile).Content);
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual((res.FirstOrDefault(f => f.Location.ToId() == "img-img1.png") as IBinarySourceFile).Content));
        }

        [Test]
        public async Task Load_IgnoreTest()
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(@"C:\site",
                new string[] 
                {
                    "*.txt",
                    "img\\*",
                    "test1.xlsx",
                    "*page2*"
                }),

            new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"C:\page2.md", null },
                { @"C:\site\folder\page2.md", null },
                { @"C:\site\page1.md", null },
                { @"C:\site\page2.html", null },
                { @"C:\site\folder\1.txt", null },
                { @"C:\site\img\img1.png", null },
                { @"C:\site\img1\img1.png", null },
                { @"C:\site\test1.xlsx", null },
            }
            ));

            var res = await loader.Load();

            Assert.AreEqual(2, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page1.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "img1-img1.png"));
        }
    }
}
