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
using System.Text;
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

            m_Composer = new BaseSiteComposer(layoutMock.Object);
        }

        [Test]
        public void ComposeSite_LayoutSimple() 
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new TextSourceFile(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l1\r\n---\r\nText Line1\r\nText Line2"),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(1, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.IsNotNull(site.MainPage.Layout);
            Assert.AreEqual("l1", site.MainPage.Layout.Name);
            Assert.AreEqual("Layout _C_", site.MainPage.Layout.RawContent);
        }

        [Test]
        public void ComposeSite_LayoutNested()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "L1 _C_"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l2.md"), "---\r\nlayout: l1\r\n---\r\nL2 _C_"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l4.md"), "---\r\nlayout: l3\r\n---\r\nL4 _C_"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l3.md"), "L3 _C_"),
                new TextSourceFile(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\n---\r\nText Line1\r\nText Line2"),
                new TextSourceFile(Location.FromPath(@"p2.md"), "---\r\nlayout: l2\r\n---\r\nP1"),
                new TextSourceFile(Location.FromPath(@"p4.md"), "---\r\nlayout: l4\r\n---\r\nP4")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.IsNull(site.MainPage.Layout);
            Assert.IsNotNull(site.MainPage.SubPages[0].Layout);
            Assert.AreEqual("l2", site.MainPage.SubPages.Find(p => p.Location.FileName == "p2.html").Layout.Name);
            Assert.AreEqual("l4", site.MainPage.SubPages.Find(p => p.Location.FileName == "p4.html").Layout.Name);
            Assert.IsNotNull(site.MainPage.SubPages.Find(p => p.Location.FileName == "p2.html").Layout.Layout);
            Assert.AreEqual("l1", site.MainPage.SubPages.Find(p => p.Location.FileName == "p2.html").Layout.Layout.Name);
            Assert.IsNotNull(site.MainPage.SubPages.Find(p => p.Location.FileName == "p4.html").Layout.Layout);
            Assert.AreEqual("l3", site.MainPage.SubPages.Find(p => p.Location.FileName == "p4.html").Layout.Layout.Name);
        }

        [Test]
        public void ComposeSite_LayoutMissing()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "Layout _C_"),
                new TextSourceFile(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l2\r\n---\r\nText Line1\r\nText Line2"),
            };

            Assert.Throws<MissingLayoutException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_DuplicateLayout()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "_C_"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.txt"), "_C_"),
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
            };

            Assert.Throws<DuplicateTemplateException>(() => m_Composer.ComposeSite(src, ""));
        }

        [Test]
        public void ComposeSite_MissingContentPLaceholderLayout()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "abc"),
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
            };

            Assert.Throws<LayoutMissingContentPlaceholderException>(() => m_Composer.ComposeSite(src, ""));
        }
    }
}
