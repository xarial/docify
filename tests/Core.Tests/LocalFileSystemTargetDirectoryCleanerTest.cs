using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Publisher;

namespace Core.Tests
{
    public class LocalFileSystemTargetDirectoryCleanerTest
    {
        [Test]
        public void CleanTest() 
        {
            var fs = new MockFileSystem();
            fs.AddDirectory("C:\\dir1");
            fs.AddDirectory("C:\\dir2");
            fs.AddDirectory("C:\\dir2\\dir3");
            fs.AddDirectory("C:\\dir2\\dir3\\dir4");
            fs.AddDirectory("C:\\dir2\\dir3\\dir4\\dir5");
            fs.AddDirectory("C:\\dir2\\dir3\\dir4\\file1.txt");
            fs.AddDirectory("C:\\dir2\\dir3\\file1.txt");
            fs.AddDirectory("C:\\dir2\\dir3\\file2.txt");
            fs.AddDirectory("C:\\dir2\\file1.txt");

            var cleaner = new LocalFileSystemTargetDirectoryCleaner(fs, true);
            cleaner.ClearDirectory(Location.FromPath("C:\\dir2\\dir3"));

            Assert.IsTrue(fs.FileExists("C:\\dir2\\file1.txt"));
            Assert.IsTrue(fs.Directory.Exists("C:\\dir2"));
            Assert.IsFalse(fs.Directory.Exists("C:\\dir2\\dir3"));
        }
    }
}
