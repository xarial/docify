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
using Xarial.Docify.Base;
using Xarial.Docify.Core;

namespace Core.Tests
{
    public class LocationTest
    {
        [Test]
        public void FromPathTest() 
        {
            var r1 = Location.FromPath(@"index.md", @"C:\MySite");
            var r2 = Location.FromPath(@"C:\MySite\page1\index.md", @"C:\MySite");
            var r3 = Location.FromPath(@"page2\index.md", @"C:\MySite");
            var r4 = Location.FromPath(@"\page3\subpage3\index.md", @"C:\MySite");

            Assert.AreEqual("index.md", r1.FileName);
            Assert.AreEqual("index.md", r2.FileName);
            Assert.AreEqual("index.md", r3.FileName);
            Assert.AreEqual("index.md", r4.FileName);
            Assert.AreEqual("index.md", r1.ToId());
            Assert.AreEqual("page1::index.md", r2.ToId());
            Assert.AreEqual("page2::index.md", r3.ToId());
            Assert.AreEqual("page3::subpage3::index.md", r4.ToId());
        }

        [Test]
        public void FromPathDirTest() 
        {
            var r1 = Location.FromPath(@"C:\dir1\dir2", @"C:\");
            var r2 = Location.FromPath(@"dir1\dir2");

            Assert.IsEmpty(r1.FileName);
            Assert.IsEmpty(r2.FileName);

            Assert.AreEqual("dir1::dir2", r1.ToId());
            Assert.AreEqual("dir1::dir2", r2.ToId());
        }

        [Test]
        public void ToUrlTest() 
        {
            var u1 = new Location("page.html", "dir1", "dir2");
            var u2 = new Location("page.html");
            var u3 = new Location("index.html", "dir1", "dir2");
            var u4 = new Location("index.html");

            var r1 = u1.ToUrl();
            var r2 = u2.ToUrl();
            var r3 = u3.ToUrl();
            var r4 = u4.ToUrl();
            var r5 = u1.ToUrl("www.site.com");
            var r6 = u2.ToUrl("www.site.com");
            var r7 = u3.ToUrl("www.site.com");
            var r8 = u4.ToUrl("www.site.com");

            Assert.AreEqual("/dir1/dir2/page.html", r1);
            Assert.AreEqual("/page.html", r2);
            Assert.AreEqual("/dir1/dir2/", r3);
            Assert.AreEqual("/", r4);
            Assert.AreEqual("www.site.com/dir1/dir2/page.html", r5);
            Assert.AreEqual("www.site.com/page.html", r6);
            Assert.AreEqual("www.site.com/dir1/dir2/", r7);
            Assert.AreEqual("www.site.com", r8);
        }

        [Test]
        public void IsFileTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");

            var r1 = l1.IsFile();
            var r2 = l2.IsFile();

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
        }

        [Test]
        public void GetParentTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");

            var r1 = l1.GetParent();
            var r2 = l2.GetParent();
            var r3 = l1.GetParent(2);
            var r4 = l2.GetParent(2);

            Assert.AreEqual("dir1::dir2", r1.ToId());
            Assert.AreEqual("dir1", r2.ToId());
            Assert.AreEqual("dir1", r3.ToId());
            Assert.AreEqual("", r4.ToId());
        }

        [Test]
        public void IsInLocationTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");
            var l3 = new Location("", "dir0", "dir1", "dir2");
            var l4 = new Location("", "dir0", "dir1", "dir2", "dir3");

            var r1 = l1.IsInLocation(l2);
            var r2 = l1.IsInLocation(l3);
            var r3 = l4.IsInLocation(l3);
            var r4 = l3.IsInLocation(l4);

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            Assert.IsTrue(r3);
            Assert.IsFalse(r4);

            Assert.Throws<Exception>(() => l2.IsInLocation(l1));
        }

        [Test]
        public void GetRelativeTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("", "dir1", "dir2");
            var l3 = new Location("", "dir0", "dir1", "dir2");
            var l4 = new Location("", "dir0", "dir1", "dir2", "dir3", "dir4");

            var r1 = l1.GetRelative(l2);
            var r2 = l4.GetRelative(l3);

            Assert.AreEqual("page.html", r1.ToId());
            Assert.AreEqual("dir3::dir4", r2.ToId());
            Assert.Throws<Exception>(() => l1.GetRelative(l3));
            Assert.Throws<Exception>(() => l3.GetRelative(l4));
        }

        [Test]
        public void IsSameTest() 
        {
            var l1 = new Location("page.html", "dir1", "dir2");
            var l2 = new Location("page.html", "Dir1", "Dir2");
            var l3 = new Location("", "Dir1", "Dir2");
            var l4 = new Location("", "dir1", "dir2");

            var r1 = l1.IsSame(l2);
            var r2 = l1.IsSame(l2, StringComparison.CurrentCulture);
            var r3 = l3.IsSame(l2);
            var r4 = l3.IsSame(l4);

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            Assert.IsFalse(r3);
            Assert.IsTrue(r4);
        }

        [Test]
        public void TestMatchPositive()
        {
            var r1 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "D:\\*" });
            var r2 = Location.FromPath("D:\\path1.txt").Matches(new string[] { ".dll", "*.txt" });
            var r3 = Location.FromPath("D:\\path1.txt1").Matches(new string[] { "*.txt" });
            var r4 = Location.FromPath("D:\\dir1\\dir2\\path1.txt").Matches(new string[] { "D:\\*\\dir2\\*" });
            var r5 = Location.FromPath("D:\\dir2\\dir3\\path1.txt").Matches(new string[] { "D:\\*\\dir2\\*" });
            var r6 = Location.FromPath("dir3\\path1.txt").Matches(new string[] { "*.*" });

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
            Assert.IsTrue(r4);
            Assert.IsFalse(r5);
            Assert.IsTrue(r6);
        }

        [Test]
        public void TestMatchNegative() 
        {
            var r1 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "|D:\\*" });
            var r2 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "|.dll", "|*.txt" });
            var r3 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "|.dll" });

            Assert.IsFalse(r1);
            Assert.IsFalse(r2);
            Assert.IsTrue(r3);
        }

        [Test]
        public void TestMatchMixed()
        {
            var r1 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "D:\\*", "|*.txt" });
            var r2 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "D:\\*", "|*.dll" });
            var r3 = Location.FromPath("D:\\path1.txt").Matches(new string[] { "C:\\*", "|*.dll" });

            Assert.IsFalse(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
        }
    }
}
