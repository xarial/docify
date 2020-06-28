//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using NUnit.Framework;
using System;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class LocationTest
    {
        [Test]
        public void FromPathTest() 
        {
            var r1 = Location.FromPath(@"index.md");
            var r2 = Location.FromPath(@"page1\index.md");
            var r3 = Location.FromPath(@"page2\index.md");
            var r4 = Location.FromPath(@"\page3\subpage3\index.md");

            Assert.AreEqual("index.md", r1.FileName);
            Assert.AreEqual("index.md", r2.FileName);
            Assert.AreEqual("index.md", r3.FileName);
            Assert.AreEqual("index.md", r4.FileName);
            Assert.AreEqual("index.md", r1.ToId());
            Assert.AreEqual("page1::index.md", r2.ToId());
            Assert.AreEqual("page2::index.md", r3.ToId());
            Assert.AreEqual("\\::page3::subpage3::index.md", r4.ToId());
        }

        [Test]
        public void FromPathDirTest() 
        {
            var r1 = Location.FromPath(@"C:\dir1\dir2");
            var r2 = Location.FromPath(@"dir1\dir2");

            Assert.IsEmpty(r1.FileName);
            Assert.IsEmpty(r2.FileName);

            Assert.AreEqual("C:\\", r1.Root);
            Assert.AreEqual("", r2.Root);
            Assert.AreEqual("C:\\::dir1::dir2", r1.ToId());
            Assert.AreEqual("dir1::dir2", r2.ToId());
        }

        [Test]
        public void ToUrlTest() 
        {
            var u1 = new Location("", "page.html", new string[] { "dir1", "dir2" });
            var u2 = new Location("", "page.html", Enumerable.Empty<string>());
            var u3 = new Location("", "index.html", new string[] { "dir1", "dir2" });
            var u4 = new Location("", "index.html", Enumerable.Empty<string>());
            var u5 = new Location("http://www.example.com", "file1.txt", Enumerable.Empty<string>());
            var u6 = new Location("https://www.example.com", "file1.txt", Enumerable.Empty<string>());

            var r1 = u1.ToUrl();
            var r2 = u2.ToUrl();
            var r3 = u3.ToUrl();
            var r4 = u4.ToUrl();
            var r5 = u5.ToUrl();
            var r6 = u6.ToUrl();

            Assert.AreEqual("/dir1/dir2/page.html", r1);
            Assert.AreEqual("/page.html", r2);
            Assert.AreEqual("/dir1/dir2/", r3);
            Assert.AreEqual("/", r4);
            Assert.AreEqual("http://www.example.com/file1.txt", r5);
            Assert.AreEqual("https://www.example.com/file1.txt", r6);
        }

        [Test]
        public void IsFileTest() 
        {
            var l1 = new Location("", "page.html", new string[] { "dir1", "dir2" });
            var l2 = new Location("", "", new string[] { "dir1", "dir2" });

            var r1 = l1.IsFile();
            var r2 = l2.IsFile();

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
        }

        [Test]
        public void GetParentTest() 
        {
            var l1 = new Location("", "page.html", new string[] { "dir1", "dir2" });
            var l2 = new Location("", "", new string[] { "dir1", "dir2" });

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
            var l1 = new Location("", "page.html", new string[] { "dir1", "dir2" });
            var l2 = new Location("", "", new string[] { "dir1", "dir2" });
            var l3 = new Location("", "", new string[] { "dir0", "dir1", "dir2" });
            var l4 = new Location("", "", new string[] { "dir0", "dir1", "dir2", "dir3" });

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
            var l1 = new Location("", "page.html", new string[] { "dir1", "dir2" });
            var l2 = new Location("", "", new string[] { "dir1", "dir2" });
            var l3 = new Location("", "", new string[] { "dir0", "dir1", "dir2" });
            var l4 = new Location("", "", new string[] { "dir0", "dir1", "dir2", "dir3", "dir4" });
            var l5 = new Location("D:\\", "f1.txt", new string[] { "dir1", "dir2" });
            var l6 = new Location("D:\\", "", new string[] { "dir1" });

            var r1 = l1.GetRelative(l2);
            var r2 = l4.GetRelative(l3);
            var r3 = l5.GetRelative(l6);

            Assert.AreEqual("page.html", r1.ToId());
            Assert.AreEqual("dir3::dir4", r2.ToId());
            Assert.AreEqual("dir2::f1.txt", r3.ToId());
            Assert.Throws<Exception>(() => l1.GetRelative(l3));
            Assert.Throws<Exception>(() => l3.GetRelative(l4));
        }

        [Test]
        public void IsSameTest() 
        {
            var l1 = new Location("", "page.html", new string[] { "dir1", "dir2" });
            var l2 = new Location("", "page.html", new string[] { "Dir1", "Dir2" });
            var l3 = new Location("", "", new string[] { "Dir1", "Dir2" });
            var l4 = new Location("", "", new string[] { "dir1", "dir2" });

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
            var r1 = Location.FromPath("dir1\\path1.txt").Matches(new string[] { "dir1\\*" });
            var r2 = Location.FromPath("dir1\\path1.txt").Matches(new string[] { ".dll", "*.txt" });
            var r3 = Location.FromPath("dir1\\path1.txt1").Matches(new string[] { "*.txt" });
            var r4 = Location.FromPath("dir0\\dir1\\dir2\\path1.txt").Matches(new string[] { "dir0\\*\\dir2\\*" });
            var r5 = Location.FromPath("dir0\\dir2\\dir3\\path1.txt").Matches(new string[] { "dir0\\*\\dir2\\*" });
            var r6 = Location.FromPath("dir3\\path1.txt").Matches(new string[] { "*.*" });

            Assert.IsTrue(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
            Assert.IsTrue(r4);
            Assert.IsFalse(r5);
            Assert.IsTrue(r6);
            Assert.Throws<Exception>(() => Location.FromPath("D:\\file1.txt").Matches(new string[] { "*" }));
        }

        [Test]
        public void TestMatchNegative() 
        {
            var r1 = Location.FromPath("dir0\\path1.txt").Matches(new string[] { "|dir0\\*" });
            var r2 = Location.FromPath("dir0\\path1.txt").Matches(new string[] { "|.dll", "|*.txt" });
            var r3 = Location.FromPath("dir0\\path1.txt").Matches(new string[] { "|.dll" });

            Assert.IsFalse(r1);
            Assert.IsFalse(r2);
            Assert.IsTrue(r3);
        }

        [Test]
        public void TestMatchMixed()
        {
            var r1 = Location.FromPath("dir0\\path1.txt").Matches(new string[] { "dir0\\*", "|*.txt" });
            var r2 = Location.FromPath("dir0\\path1.txt").Matches(new string[] { "dir0\\*", "|*.dll" });
            var r3 = Location.FromPath("dir0\\path1.txt").Matches(new string[] { "dir1\\*", "|*.dll" });

            Assert.IsFalse(r1);
            Assert.IsTrue(r2);
            Assert.IsFalse(r3);
        }

        [Test]
        public void FromStringTest()
        {
            var r1 = Location.FromString("abc\\xyz");
            var r2 = Location.FromString("abc/xyz");
            var r4 = Location.FromString("abc\\xyz\\file1.txt");
            var r5 = Location.FromString("abc/xyz/file1.txt");
            var r6 = Location.FromString("D:\\dir\\file1.txt");
            var r7 = Location.FromString("https://www.example.com/file1.txt");
            var r8 = Location.FromString("https://www.example.com:8080/file1.txt");
            var r9 = Location.FromString("/file1.txt");
            var r10 = Location.FromString("\\file1.txt");
            var r11 = Location.FromString("/");
            var r12 = Location.FromString("abc::xyz::file1.txt");
            var r13 = Location.FromString("::file1.txt");
            var r14 = Location.FromString("::");

            Assert.AreEqual("", r1.FileName);
            Assert.AreEqual("", r2.FileName);
            Assert.AreEqual("file1.txt", r4.FileName);
            Assert.AreEqual("file1.txt", r5.FileName);
            Assert.AreEqual("file1.txt", r6.FileName);
            Assert.AreEqual("file1.txt", r7.FileName);
            Assert.AreEqual("file1.txt", r8.FileName);
            Assert.AreEqual("file1.txt", r9.FileName);
            Assert.AreEqual("file1.txt", r10.FileName);
            Assert.AreEqual("", r11.FileName);
            Assert.AreEqual("file1.txt", r12.FileName);
            Assert.AreEqual("file1.txt", r13.FileName);
            Assert.AreEqual("", r14.FileName);

            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r1.Segments));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r2.Segments));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r4.Segments));
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r5.Segments));
            Assert.IsTrue(new string[] { "dir" }.SequenceEqual(r6.Segments));
            Assert.IsFalse(r7.Segments.Any());
            Assert.IsFalse(r8.Segments.Any());
            Assert.IsFalse(r9.Segments.Any());
            Assert.IsFalse(r10.Segments.Any());
            Assert.IsFalse(r11.Segments.Any());
            Assert.IsTrue(new string[] { "abc", "xyz" }.SequenceEqual(r12.Segments));
            Assert.IsFalse(r13.Segments.Any());
            Assert.IsFalse(r14.Segments.Any());

            Assert.AreEqual("", r1.Root);
            Assert.AreEqual("", r2.Root);
            Assert.AreEqual("", r4.Root);
            Assert.AreEqual("", r5.Root);
            Assert.AreEqual("D:\\", r6.Root);
            Assert.AreEqual("https://www.example.com", r7.Root);
            Assert.AreEqual("https://www.example.com:8080", r8.Root);
            Assert.AreEqual("/", r9.Root);
            Assert.AreEqual("\\", r10.Root);
            Assert.AreEqual("/", r11.Root);
            Assert.AreEqual("", r12.Root);
            Assert.AreEqual("::", r13.Root);
            Assert.AreEqual("::", r14.Root);
        }
    }
}
