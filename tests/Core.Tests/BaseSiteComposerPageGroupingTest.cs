//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Tests.Common.Mocks;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Core.Tests
{
    public class BaseSiteComposerPageGroupingTest
    {
        private BaseSiteComposer m_Composer;

        [SetUp]
        public void Setup()
        {
            m_Composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);
        }

        [Test]
        public async Task ComposeSite_IndexPageTest() 
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1\index.md"), "p1")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("", site.MainPage.Name);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
        }

        [Test]
        public async Task ComposeSite_NamedPageTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1.md"), "p1")
            }.ToAsyncEnumerable();
            
            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
        }

        [Test]
        public async Task ComposeSite_NestedIndexPageUndefinedTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1\page2\index.md"), "p2")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.SubPages[0].RawContent));
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page2", site.MainPage.SubPages[0].SubPages[0].Name);
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
        }

        [Test]
        public async Task ComposeSite_NestedIndexPageTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1\page2\index.md"), "p2"),
                new FileMock(Location.FromPath(@"page1\index.md"), "p1")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page2", site.MainPage.SubPages[0].SubPages[0].Name);
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
        }

        [Test]
        public async Task ComposeSite_NestedNamedPageUndefinedTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1\page2.md"), "p2")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.IsNotInstanceOf<PhantomPage>(site.MainPage);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.IsInstanceOf<PhantomPage>(site.MainPage.SubPages[0]);
            Assert.IsTrue(string.IsNullOrEmpty(site.MainPage.SubPages[0].RawContent));
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page2", site.MainPage.SubPages[0].SubPages[0].Name);
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
            Assert.IsNotInstanceOf<PhantomPage>(site.MainPage.SubPages[0].SubPages[0]);
        }

        [Test]
        public async Task ComposeSite_NestedNamedPageTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1\index.md"), "p1"),
                new FileMock(Location.FromPath(@"page1\page2.md"), "p2")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
            Assert.AreEqual(1, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page2", site.MainPage.SubPages[0].SubPages[0].Name);
            Assert.AreEqual("p2", site.MainPage.SubPages[0].SubPages[0].RawContent);
        }

        [Test]
        public void ComposeSite_DuplicateTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1.md"), "")
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<DuplicatePageException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public async Task ComposeSite_CaseInsensitiveTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"PAGE1\Page2\index.md"), "p2"),
                new FileMock(Location.FromPath(@"page1\index.md"), "p1"),
                new FileMock(Location.FromPath(@"page1\Page3\INDEX.md"), "p3"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Name.Equals("page1", StringComparison.CurrentCultureIgnoreCase));
            var p2 = p1?.SubPages.First(p => p.Name.Equals("page2", StringComparison.CurrentCultureIgnoreCase));
            var p3 = p1?.SubPages.First(p => p.Name.Equals("page3", StringComparison.CurrentCultureIgnoreCase));

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.IsNotNull(p1);
            Assert.AreEqual("p1", p1.RawContent);
            Assert.AreEqual(2, p1.SubPages.Count);
            Assert.IsNotNull(p2);
            Assert.AreEqual("p2", p2.RawContent);
            Assert.IsNotNull(p3);
            Assert.AreEqual("p3", p3.RawContent);
        }

        [Test]
        public void ComposeSite_EmptySiteTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"page1\index.txt"), ""),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<EmptySiteException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_NoMainPageTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1.md"), "")
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<SiteMainPageMissingException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public async Task ComposeSite_DifferentPageTypesTest()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.html"), "i"),
                new FileMock(Location.FromPath(@"page1\page2\index.cshtml"), "p2"),
                new FileMock(Location.FromPath(@"page1\index.md"), "p1"),
                new FileMock(Location.FromPath(@"page1\page3\index.html"), "p3"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Name.Equals("page1", StringComparison.CurrentCultureIgnoreCase));
            var p2 = p1?.SubPages.First(p => p.Name.Equals("page2", StringComparison.CurrentCultureIgnoreCase));
            var p3 = p1?.SubPages.First(p => p.Name.Equals("page3", StringComparison.CurrentCultureIgnoreCase));

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.IsNotNull(p1);
            Assert.AreEqual("p1", p1.RawContent);
            Assert.AreEqual(2, p1.SubPages.Count);
            Assert.IsNotNull(p2);
            Assert.AreEqual("p2", p2.RawContent);
            Assert.IsNotNull(p3);
            Assert.AreEqual("p3", p3.RawContent);
        }

        [Test]
        public async Task ComposeSite_SkippedNotPageTest()
        {
            var src = new IFile[]
            {
                new FileMock(Location.FromPath(@"index.md"), "i"),
                new FileMock(Location.FromPath(@"page1\index.md"), "p1"),
                new FileMock(Location.FromPath(@"page1\asset1.txt"), "p1"),
                new FileMock(Location.FromPath(@"asset2.ini"), "p1")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.SubPages.Count);
            Assert.AreEqual("", site.MainPage.Name);
            Assert.AreEqual("i", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.SubPages[0].SubPages.Count);
            Assert.AreEqual("page1", site.MainPage.SubPages[0].Name);
            Assert.AreEqual("p1", site.MainPage.SubPages[0].RawContent);
        }
    }
}
