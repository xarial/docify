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
using Xarial.Docify.Base;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;

namespace Core.Tests
{
    public class LocalFileSystemFileLoaderTest
    {
        [Test]
        public async Task LoadFolder_TextAndBinaryTest() 
        {
            var loader = new LocalFileSystemFileLoader(
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

            var res = await loader.LoadFolder(Location.FromPath("C:\\site"), null).ToListAsync();

            Assert.AreEqual(5, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page1.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page2.html"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "folder::1.txt"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "img::img1.png"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "test1.xlsx"));
            Assert.AreEqual("abc", res.FirstOrDefault(f => f.Location.ToId() == "page1.md").AsTextContent());
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(res.FirstOrDefault(f => f.Location.ToId() == "img::img1.png").Content));
        }

        [Test]
        public async Task LoadFolder_IgnoreTest()
        {
            var loader = new LocalFileSystemFileLoader(
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

            var res = await loader.LoadFolder(Location.FromPath("C:\\site"),
                new string[]
                {
                    "|*.txt",
                    "|img\\*",
                    "|test1.xlsx",
                    "|*page2*"
                }).ToListAsync();

            Assert.AreEqual(2, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page1.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "img1::img1.png"));
        }

        [Test]
        public void LoadFolder_MissingLocation() 
        {
            var loader = new LocalFileSystemFileLoader(
                new MockFileSystem(new Dictionary<string, MockFileData>()
                {
                    { @"C:\page2.md", null },
                    { @"C:\site\folder\page2.md", null }
                }));

            Assert.Throws<MissingLocationException>(() => loader.LoadFolder(Location.FromPath("C:\\site1"), null).ToEnumerable().ToList());
        }
    }
}
