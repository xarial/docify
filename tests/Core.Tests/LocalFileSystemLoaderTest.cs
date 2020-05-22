﻿//*********************************************************************
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
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;

namespace Core.Tests
{
    public static class LocalFileSystemLoaderExtension
    {
        public static IAsyncEnumerable<IFile> Load(this LocalFileSystemLoader loader, ILocation location)
        {
            return loader.Load(new ILocation[] { location });
        }
    }

    public class LocalFileSystemLoaderTest
    {
        [Test]
        public async Task Load_TextAndBinaryTest() 
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(new string[0].ToList()),
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

            var res = await loader.Load(Location.FromPath("C:\\site")).ToListAsync();

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
        public async Task Load_IgnoreTest()
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(
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

            var res = await loader.Load(Location.FromPath("C:\\site")).ToListAsync();

            Assert.AreEqual(2, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page1.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "img1::img1.png"));
        }

        [Test]
        public async Task Load_MultipleLocationsTest() 
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(new string[0].ToList()),
            new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"C:\site\index.md", new MockFileData("abc") },
                { @"C:\site\1.txt", null },
                { @"C:\site1\page2\index.md", new MockFileData("xyz") },
                { @"C:\site1\page2\2.txt", null }
            }));

            var res = await loader.Load(new ILocation[] { Location.FromPath("C:\\site"), Location.FromPath("C:\\site1") }).ToListAsync();

            Assert.AreEqual(4, res.Count());
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "index.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "1.txt"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page2::index.md"));
            Assert.IsNotNull(res.FirstOrDefault(f => f.Location.ToId() == "page2::2.txt"));
        }

        [Test]
        public async Task Load_MultipleLocationsConflictTest()
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(new string[0].ToList()),
            new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"C:\site\index.md", new MockFileData("abc") },
                { @"C:\site1\index.md", new MockFileData("xyz") },
            }));

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

        [Test]
        public async Task Load_MissingLocation() 
        {
            var loader = new LocalFileSystemLoader(new LocalFileSystemLoaderConfig(Enumerable.Empty<string>()),
            new MockFileSystem(new Dictionary<string, MockFileData>()
            {
                { @"C:\page2.md", null },
                { @"C:\site\folder\page2.md", null }
            }));

            Exception err = null;

            //cannot test IAsyncEnumerable with Assert.Throws or Assert.ThrowsAsync
            try
            {
                await foreach (var x in loader.Load(Location.FromPath("C:\\site1"))) ;
            }
            catch (MissingLocationException ex)
            {
                err = ex;
            }

            Assert.IsNotNull(err);
        }

        [Test]
        public void LocalFileSystemLoaderConfig_IgnoreEmptyTest()
        {
            var config = new LocalFileSystemLoaderConfig(
                new Configuration());

            Assert.IsEmpty(config.Ignore);
        }

        [Test]
        public void LocalFileSystemLoaderConfig_IgnoreTest()
        {
            var config = new LocalFileSystemLoaderConfig(
                new Configuration(new Dictionary<string, dynamic>()
                {
                    {
                        "ignore", new List<string>(new string[] { "A", "B" })
                    }
                }));

            Assert.AreEqual(2, config.Ignore.Count);
            Assert.Contains("A", config.Ignore);
            Assert.Contains("B", config.Ignore);
        }

        [Test]
        public void LocalFileSystemLoaderConfig_IgnoreInvalidCastTest()
        {
            Assert.Throws<InvalidCastException>(() => new LocalFileSystemLoaderConfig(
                    new Configuration(new Dictionary<string, dynamic>()
                    {
                        {
                            "ignore", "A"
                        }
                    })));
        }
    }
}
