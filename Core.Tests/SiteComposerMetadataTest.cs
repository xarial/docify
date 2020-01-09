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
    public class SiteComposerMetadataTest
    {
        private SiteComposer NewComposer()
        {
            return new SiteComposer(new LayoutParser());
        }

        [Test]
        public void ComposeSite_ContentMetadataSimpleProperties()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"),
                "---\r\nprp1: A\r\nprp2: B\r\n---\r\nText Line1\r\nText Line2"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual("B", site.MainPage.Data["prp2"]);
        }

        [Test]
        public void ComposeSite_ContentMetadataNestedProperties()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"),
                "---\r\nprp1: A\r\nprp2:\r\n  prp3: B\r\n---\r\nText Line1\r\nText Line2"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual(1, site.MainPage.Data["prp2"].Count);
            Assert.AreEqual("B", site.MainPage.Data["prp2"]["prp3"]);
        }

        [Test]
        public void ComposeSite_ContentMetadataArray()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"),
                "---\r\nprp1: A\r\nprp2:\r\n  - B\r\n  - C\r\n---\r\nText Line1\r\nText Line2"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual(2, site.MainPage.Data["prp2"].Count);
            Assert.AreEqual("B", site.MainPage.Data["prp2"][0]);
            Assert.AreEqual("C", site.MainPage.Data["prp2"][1]);
        }

        [Test]
        public void ComposeSite_ContentNoFrontMatter()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"),
                "Text Line1\r\nText Line2"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.Data.Count);
        }

        [Test]
        public void ComposeSite_NotClosedFrontMatter()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"),
                "---\r\nText Line1\r\nText Line2"),
            };

            var composer = NewComposer();

            Assert.Throws<FrontMatterErrorException>(() => composer.ComposeSite(src, ""));
        }
    }
}
