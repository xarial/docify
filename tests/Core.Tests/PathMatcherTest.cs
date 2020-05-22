using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core.Helpers;

namespace Core.Tests
{
    public class PathMatcherTest
    {
        [Test]
        public void TestMatchPath()
        {
            var r1 = PathMatcher.Matches(new string[] { "D:\\*" }, "D:\\path1.txt");
            var r2 = PathMatcher.Matches(new string[] { ".dll", "*.txt" }, "D:\\path1.txt");
            var r3 = PathMatcher.Matches(new string[] { "*.txt" }, "D:\\path1.txt1");
            var r4 = PathMatcher.Matches(new string[] { "D:\\*\\dir2\\*" }, "D:\\dir1\\dir2\\path1.txt");
            var r5 = PathMatcher.Matches(new string[] { "D:\\*\\dir2\\*" }, "D:\\dir2\\dir3\\path1.txt");
            var r6 = PathMatcher.Matches(new string[] { "*.*" }, "dir3\\path1.txt");

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
            Assert.IsTrue(r4);
            Assert.IsFalse(r5);
            Assert.IsTrue(r6);
        }
    }
}
