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
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class SiteComposerLayoutTest
    {
        [Test]
        public void ComposeSite_LayoutSimple() 
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "Layout {{ content }}"),
                new TextSourceFile(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l1\r\n---\r\nText Line1\r\nText Line2"),
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(1, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.IsNotNull(site.MainPage.Layout);
            Assert.AreEqual("l1", site.MainPage.Layout.Name);
            Assert.AreEqual("Layout {{ content }}", site.MainPage.Layout.RawContent);
        }

        [Test]
        public void ComposeSite_LayoutNested()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "L1 {{ content }}"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l2.md"), "---\r\nlayout: l1\r\n---\r\nL2 {{ content }}"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l4.md"), "---\r\nlayout: l3\r\n---\r\nL4 {{ content }}"),
                new TextSourceFile(Location.FromPath(@"_layouts\\l3.md"), "L3 {{ content }}"),
                new TextSourceFile(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\n---\r\nText Line1\r\nText Line2"),
                new TextSourceFile(Location.FromPath(@"p2.md"), "---\r\nlayout: l2\r\n---\r\nP1"),
                new TextSourceFile(Location.FromPath(@"p4.md"), "---\r\nlayout: l4\r\n---\r\nP4")
            };

            var composer = new SiteComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.IsNull(site.MainPage.Layout);
            Assert.IsNotNull(site.MainPage.Children[0].Layout);
            Assert.AreEqual("l2", site.MainPage.Children.Find(p => p.Location.FileName == "p2.html").Layout.Name);
            Assert.AreEqual("l4", site.MainPage.Children.Find(p => p.Location.FileName == "p4.html").Layout.Name);
            Assert.IsNotNull(site.MainPage.Children.Find(p => p.Location.FileName == "p2.html").Layout.Layout);
            Assert.AreEqual("l1", site.MainPage.Children.Find(p => p.Location.FileName == "p2.html").Layout.Layout.Name);
            Assert.IsNotNull(site.MainPage.Children.Find(p => p.Location.FileName == "p4.html").Layout.Layout);
            Assert.AreEqual("l3", site.MainPage.Children.Find(p => p.Location.FileName == "p4.html").Layout.Layout.Name);
        }

        [Test]
        public void ComposeSite_LayoutMissing()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_layouts\\l1.md"), "Layout {{ content }}"),
                new TextSourceFile(Location.FromPath(@"index.md"),
                    "---\r\nprp1: A\r\nlayout: l2\r\n---\r\nText Line1\r\nText Line2"),
            };

            var composer = new SiteComposer();

            Assert.Throws<MissingLayoutException>(() => composer.ComposeSite(src, ""));
        }
    }
}
