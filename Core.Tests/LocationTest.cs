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
    }
}
