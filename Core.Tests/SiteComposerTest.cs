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
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class SiteComposerTest
    {
        [Test]
        public void ComposeSite_IndexPageTest() 
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1\index.md"), "p1")
            };
            
            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("index.html", site.MainPage.Location.ToId());
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
        }

        [Test]
        public void ComposeSite_NamedPageTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1.md"), "p1")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedIndexPageUndefinedTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1\page2\index.md"), "p2")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.Children[0].RawContent));
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2-index.html", site.MainPage.Children[0].Children[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.Children[0].Children[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedIndexPageTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1\page2\index.md"), "p2"),
                new SourceFile(Location.FromPath(@"page1\index.md"), "p1")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2-index.html", site.MainPage.Children[0].Children[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.Children[0].Children[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedNamedPageUndefinedTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1\page2.md"), "p2")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.Children[0].RawContent));
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2.html", site.MainPage.Children[0].Children[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.Children[0].Children[0].RawContent);
        }

        [Test]
        public void ComposeSite_NestedNamedPageTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1\index.md"), "p1"),
                new SourceFile(Location.FromPath(@"page1\page2.md"), "p2")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2.html", site.MainPage.Children[0].Children[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.Children[0].Children[0].RawContent);
        }

        [Test]
        public void ComposeSite_DuplicateTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), ""),
                new SourceFile(Location.FromPath(@"page1\index.md"), ""),
                new SourceFile(Location.FromPath(@"page1.md"), "")
            };

            var composer = new SiteComposer();

            Assert.Throws<DuplicatePageException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_CaseInsensitiveTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"PAGE1\Page2\index.md"), "p2"),
                new SourceFile(Location.FromPath(@"page1\index.md"), "p1"),
                new SourceFile(Location.FromPath(@"page1\Page3\INDEX.md"), "p3"),
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            StringAssert.AreEqualIgnoringCase("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
            Assert.AreEqual(2, site.MainPage.Children[0].Children.Count);
            StringAssert.AreEqualIgnoringCase("page1-page2-index.html", site.MainPage.Children[0].Children[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.Children[0].Children[0].RawContent);
            StringAssert.AreEqualIgnoringCase("page1-page3-index.html", site.MainPage.Children[0].Children[1].Location.ToId());
            Assert.AreEqual("p3", site.MainPage.Children[0].Children[1].RawContent);
        }

        [Test]
        public void ComposeSite_EmptySiteTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"page1\index.txt"), ""),
            };

            var composer = new SiteComposer();

            Assert.Throws<EmptySiteException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_NoMainPageTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"page1\index.md"), ""),
                new SourceFile(Location.FromPath(@"page1.md"), "")
            };

            var composer = new SiteComposer();

            Assert.Throws<SiteMainPageMissingException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_DifferentPageTypesTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.html"), "i"),
                new SourceFile(Location.FromPath(@"page1\page2\index.cshtml"), "p2"),
                new SourceFile(Location.FromPath(@"page1\index.md"), "p1"),
                new SourceFile(Location.FromPath(@"page1\page3\index.html"), "p3"),
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
            Assert.AreEqual(2, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2-index.html", site.MainPage.Children[0].Children[0].Location.ToId());
            Assert.AreEqual("p2", site.MainPage.Children[0].Children[0].RawContent);
            Assert.AreEqual("page1-page3-index.html", site.MainPage.Children[0].Children[1].Location.ToId());
            Assert.AreEqual("p3", site.MainPage.Children[0].Children[1].RawContent);
        }

        [Test]
        public void ComposeSite_SkippedNotPageTest()
        {
            var src = new SourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md"), "i"),
                new SourceFile(Location.FromPath(@"page1\index.md"), "p1"),
                new SourceFile(Location.FromPath(@"page1\asset1.txt"), "p1"),
                new SourceFile(Location.FromPath(@"asset2.ini"), "p1")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Children.Count);
            Assert.AreEqual("index.html", site.MainPage.Location.ToId());
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Location.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
        }
    }
}
