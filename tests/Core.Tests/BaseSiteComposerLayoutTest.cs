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
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Composer;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Core.Tests
{
    public class BaseSiteComposerLayoutTest
    {
        [Test]
        public async Task ComposeSite_LayoutSimple() 
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new FileMock(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l1\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await composer.ComposeSite(src, "");

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
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

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

            var site = await composer.ComposeSite(src, "");

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
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new FileMock(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l2\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<MissingLayoutException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_DuplicateLayout()
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "_C_"),
                new FileMock(Location.FromPath(@"_layouts\\l1.txt"), "_C_"),
                new FileMock(Location.FromPath(@"index.md"), ""),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<DuplicateTemplateException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public async Task ComposeSite_LayoutMetadata()
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "---\r\nprp1: B\r\nprp2: C\r\n---\r\nLayout _C_"),
                new FileMock(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l1\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await composer.ComposeSite(src, "");

            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual("C", site.MainPage.Data["prp2"]);
        }

        [Test]
        public void ComposeSite_MissingContentPlaceholderLayout()
        {
            var layoutMock = new Mock<ILayoutParser>();

            layoutMock.Setup(m => m.ValidateLayout(It.IsAny<string>()))
                .Callback<string>(c => throw new Exception());

            var composer = new BaseSiteComposer(layoutMock.Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "abc"),
                new FileMock(Location.FromPath(@"index.md"), ""),
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<InvalidLayoutException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public async Task ComposeSite_SubFolderLayout()
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\dir1\\l1.md"), "_C_"),
                new FileMock(Location.FromPath(@"index.md"), ""),
            }.ToAsyncEnumerable();

            var site = await composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.Layouts.Count);
            Assert.AreEqual("dir1::l1", site.Layouts[0].Name);
            Assert.AreEqual("_C_", site.Layouts[0].RawContent);
        }

        [Test]
        public void ComposeSite_InheritLayoutMissing()
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\\index.md"), "---\r\nlayout: $\r\n---")
            }.ToAsyncEnumerable();

            Assert.ThrowsAsync<MissingInheritLayoutException>(() => composer.ComposeSite(src, ""));
        }

        [Test]
        public async Task ComposeSite_InheritLayout()
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new FileMock(Location.FromPath(@"index.md"), "---\r\nlayout: l1\r\n---"),
                new FileMock(Location.FromPath(@"page1\\index.md"), "---\r\nlayout: $\r\n---"),
            }.ToAsyncEnumerable();

            var site = await composer.ComposeSite(src, "");

            Assert.AreEqual("l1", site.MainPage.SubPages.First(p => p.Name == "page1").Layout.Name);
        }

        [Test]
        public async Task ComposeSite_MultiLevelInheritLayout()
        {
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new FileMock(Location.FromPath(@"index.md"), "---\r\nlayout: l1\r\n---"),
                new FileMock(Location.FromPath(@"page1\\index.md"), "---\r\nlayout: $\r\n---"),
                new FileMock(Location.FromPath(@"page1\\page2\\index.md"), "---\r\nlayout: $\r\n---"),
            }.ToAsyncEnumerable();

            var site = await composer.ComposeSite(src, "");

            Assert.AreEqual("l1", site.MainPage.SubPages.First(p => p.Name == "page1").SubPages.First(p => p.Name == "page2").Layout.Name);
        }

        [Test]
        public async Task ComposeSiteDefaultLayout() 
        {
            var conf = new Configuration() { { "default-layout", "l1" } };
            var composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, conf, new Mock<IComposerExtension>().Object);

            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"_layouts\\l1.md"), ""),
                new FileMock(Location.FromPath(@"_layouts\\l2.md"), ""),
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\\index.md"), ""),
                new FileMock(Location.FromPath(@"page2\\index.md"), "---\r\nlayout: l2\r\n---"),
            }.ToAsyncEnumerable();

            var site = await composer.ComposeSite(src, "");

            Assert.AreEqual("l1", site.MainPage.Layout.Name);
            Assert.AreEqual("l1", site.MainPage.SubPages.First(p => p.Name == "page1").Layout.Name);
            Assert.AreEqual("l2", site.MainPage.SubPages.First(p => p.Name == "page2").Layout.Name);
        }
    }
}
