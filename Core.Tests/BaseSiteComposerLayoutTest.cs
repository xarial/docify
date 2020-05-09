//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class BaseSiteComposerLayoutTest
    {
        private BaseSiteComposer m_Composer;

        [SetUp]
        public void Setup()
        {
            var layoutMock = new Mock<ILayoutParser>();

            layoutMock.Setup(m => m.ContainsPlaceholder(It.IsAny<string>()))
                .Returns<string>(c => c.Contains("_C_"));

            m_Composer = new BaseSiteComposer(layoutMock.Object, null);
        }

        [Test]
        public async Task ComposeSite_LayoutSimple() 
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new FileMock(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l1\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(1, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.IsNotNull(site.MainPage.Layout);
            Assert.AreEqual("l1", site.MainPage.Layout.Name);
            Assert.AreEqual("Layout _C_", site.MainPage.Layout.RawContent);
        }

        [Test]
        public async Task ComposeSite_LayoutNested()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "L1 _C_"),
                new FileMock(Location.FromPath(@"_layouts\\l2.md"), "---\r\nlayout: l1\r\n---\r\nL2 _C_"),
                new FileMock(Location.FromPath(@"_layouts\\l4.md"), "---\r\nlayout: l3\r\n---\r\nL4 _C_"),
                new FileMock(Location.FromPath(@"_layouts\\l3.md"), "L3 _C_"),
                new FileMock(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\n---\r\nText Line1\r\nText Line2"),
                new FileMock(Location.FromPath(@"p2.md"), "---\r\nlayout: l2\r\n---\r\nP1"),
                new FileMock(Location.FromPath(@"p4.md"), "---\r\nlayout: l4\r\n---\r\nP4")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.IsNull(site.MainPage.Layout);
            Assert.IsNotNull(site.MainPage.SubPages[0].Layout);
            Assert.AreEqual("l2", site.MainPage.SubPages.Find(p => p.Name == "p2").Layout.Name);
            Assert.AreEqual("l4", site.MainPage.SubPages.Find(p => p.Name == "p4").Layout.Name);
            Assert.IsNotNull(site.MainPage.SubPages.Find(p => p.Name == "p2").Layout.Layout);
            Assert.AreEqual("l1", site.MainPage.SubPages.Find(p => p.Name == "p2").Layout.Layout.Name);
            Assert.IsNotNull(site.MainPage.SubPages.Find(p => p.Name == "p4").Layout.Layout);
            Assert.AreEqual("l3", site.MainPage.SubPages.Find(p => p.Name == "p4").Layout.Layout.Name);
        }

        [Test]
        public void ComposeSite_LayoutMissing()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new FileMock(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l2\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<MissingLayoutException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_DuplicateLayout()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "_C_"),
                new FileMock(Location.FromPath(@"_layouts\\l1.txt"), "_C_"),
                new FileMock(Location.FromPath(@"index.md"), ""),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<DuplicateTemplateException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_MissingContentPLaceholderLayout()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "abc"),
                new FileMock(Location.FromPath(@"index.md"), ""),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<LayoutMissingContentPlaceholderException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public async Task ComposeSite_SubFolderLayout()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\dir1\\l1.md"), "_C_"),
                new FileMock(Location.FromPath(@"index.md"), ""),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.Layouts.Count);
            Assert.AreEqual("dir1::l1", site.Layouts[0].Name);
            Assert.AreEqual("_C_", site.Layouts[0].RawContent);
        }
    }
}
