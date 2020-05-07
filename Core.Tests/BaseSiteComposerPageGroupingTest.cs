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
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class BaseSiteComposerPageGroupingTest
    {
        private BaseSiteComposer m_Composer;

        [SetUp]
        public void Setup()
        {
            m_Composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null);
        }

        [Test]
        public void ComposeSite_IndexPageTest() 
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1\index.md"), "p1")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("index.html", site.MainPage.Location.ToId());
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
        }

        [Test]
        public void ComposeSite_NamedPageTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1.md"), "p1")
            };
            
            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedIndexPageUndefinedTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1\page2\index.md"), "p2")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.SubPages[0].RawContent));
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::page2::index.html", site.MainPage.SubPages[0].SubPages[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedIndexPageTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1\page2\index.md"), "p2"),
                new File(Location.FromPath(@"page1\index.md"), "p1")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::page2::index.html", site.MainPage.SubPages[0].SubPages[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedNamedPageUndefinedTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1\page2.md"), "p2")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.IsNotInstanceOf<PhantomPage>(site.MainPage);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.IsInstanceOf<PhantomPage>(site.MainPage.SubPages[0]);
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.SubPages[0].RawContent));
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::page2.html", site.MainPage.SubPages[0].SubPages[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
            Assert.IsNotInstanceOf<PhantomPage>(site.MainPage.SubPages[0].SubPages[0]);
        }

        [Test]
        public void ComposeSite_NestedNamedPageTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1\index.md"), "p1"),
                new File(Location.FromPath(@"page1\page2.md"), "p2")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::page2.html", site.MainPage.SubPages[0].SubPages[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
        }

        [Test]
        public void ComposeSite_DuplicateTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1.md"), "")
            };

            Assert.Throws<DuplicatePageException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_CaseInsensitiveTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"PAGE1\Page2\index.md"), "p2"),
                new File(Location.FromPath(@"page1\index.md"), "p1"),
                new File(Location.FromPath(@"page1\Page3\INDEX.md"), "p3"),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            StringAssert.AreEqualIgnoringCase("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
            Assert.AreEqual(2, site.MainPage.SubPages[0].SubPages.Count);
            StringAssert.AreEqualIgnoringCase("page1::page2::index.html", site.MainPage.SubPages[0].SubPages[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
            StringAssert.AreEqualIgnoringCase("page1::page3::index.html", site.MainPage.SubPages[0].SubPages[1].Location.ToId());
            Assert.AreEqual("p3", site.MainPage.SubPages[0].SubPages[1].RawContent);
        }

        [Test]
        public void ComposeSite_EmptySiteTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"page1\index.txt"), ""),
            };

            Assert.Throws<EmptySiteException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_NoMainPageTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1.md"), "")
            };

            Assert.Throws<SiteMainPageMissingException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_DifferentPageTypesTest()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.html"), "i"),
                new File(Location.FromPath(@"page1\page2\index.cshtml"), "p2"),
                new File(Location.FromPath(@"page1\index.md"), "p1"),
                new File(Location.FromPath(@"page1\page3\index.html"), "p3"),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
            Assert.AreEqual(2, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::page2::index.html", site.MainPage.SubPages[0].SubPages[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
            Assert.AreEqual("page1::page3::index.html", site.MainPage.SubPages[0].SubPages[1].Location.ToId());
            Assert.AreEqual("p3", site.MainPage.SubPages[0].SubPages[1].RawContent);
        }

        [Test]
        public void ComposeSite_SkippedNotPageTest()
        {
            var src = new IFile[]
            {
                new File(Location.FromPath(@"index.md"), "i"),
                new File(Location.FromPath(@"page1\index.md"), "p1"),
                new File(Location.FromPath(@"page1\asset1.txt"), "p1"),
                new File(Location.FromPath(@"asset2.ini"), "p1")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("index.html", site.MainPage.Location.ToId());
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1::index.html", site.MainPage.SubPages[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
        }
    }
}
