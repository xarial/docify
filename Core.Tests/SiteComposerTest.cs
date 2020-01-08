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
            Assert.AreEqual("index.html", site.MainPage.Url.ToId());
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Url.ToId());
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
            Assert.AreEqual("page1.html", site.MainPage.Children[0].Url.ToId());
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
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Url.ToId());
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.Children[0].RawContent));
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2-index.html", site.MainPage.Children[0].Children[0].Url.ToId());
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
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Url.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2-index.html", site.MainPage.Children[0].Children[0].Url.ToId());
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
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Url.ToId());
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.Children[0].RawContent));
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2.html", site.MainPage.Children[0].Children[0].Url.ToId());
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
            Assert.AreEqual("page1-index.html", site.MainPage.Children[0].Url.ToId());
            Assert.AreEqual("p1", site.MainPage.Children[0].RawContent);
            Assert.AreEqual(1, site.MainPage.Children[0].Children.Count);
            Assert.AreEqual("page1-page2.html", site.MainPage.Children[0].Children[0].Url.ToId());
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
        public void ComposeSite_MultiLevelIndexOnlyTest()
        {
            var elems = new ISourceFile[]
            {
                new SourceFile(Location.FromPath(@"index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"C:\MySite\page1\index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"page2\index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"C:\MySite\page2\subpage1\index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"page2\subpage1\subsubpage1\index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"C:\MySite\page1\subpage2\index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"\page3\subpage3\index.md", @"C:\MySite"), ""),
                new SourceFile(Location.FromPath(@"C:\MySite\page2\subpage1\subsubpage2\index.md", @"C:\MySite"), ""),
            };
            
            var composer = new SiteComposer();
            var site = composer.ComposeSite(elems, "");

            //Assert.AreEqual(3, site.Pages.Count());
            //Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page1"));
            //Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page2"));
            //Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page3"));

            //Assert.AreEqual(1, site.Pages.First(p => p.Url == "page1").Children.Count());
            //Assert.IsNotNull(site.Pages.First(p => p.Url == "page1").Children.FirstOrDefault(p => p.Url == "page1/subpage2"));

            //Assert.AreEqual(1, site.Pages.First(p => p.Url == "page2").Children.Count());
            //Assert.AreEqual(2, site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.Count());
            //Assert.IsNotNull(site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.FirstOrDefault(p => p.Url == "page2/subpage1/subsubpage1"));
            //Assert.IsNotNull(site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.FirstOrDefault(p => p.Url == "page2/subpage1/subsubpage2"));

            //Assert.AreEqual(1, site.Pages.First(p => p.Url == "page3").Children.Count());
            //Assert.IsNotNull(site.Pages.First(p => p.Url == "page3").Children.FirstOrDefault(p => p.Url == "page3/subpage3"));
        }
    }
}
