//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Exceptions;
using Moq;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Composer;
using System.Linq;
using System.Threading.Tasks;
using Tests.Common.Mocks;
using Xarial.Docify.Core.Plugin.Extensions;
using Tests.Common;

namespace Core.Tests
{
    public class BaseSiteComposerMetadataTest
    {
        private BaseSiteComposer m_Composer;

        [SetUp]
        public void Setup()
        {
            m_Composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, new Mock<IComposerExtension>().Object);
        }

        [Test]
        public async Task ComposeSite_ContentMetadataSimpleProperties()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"),
                "---\r\nprp1: A\r\nprp2: B\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual("B", site.MainPage.Data["prp2"]);
        }

        [Test]
        public async Task ComposeSite_ContentMetadataNestedProperties()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"),
                "---\r\nprp1: A\r\nprp2:\r\n  prp3: B\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual(1, (site.MainPage.Data["prp2"] as System.Collections.IDictionary).Count);
            Assert.AreEqual("B", (site.MainPage.Data["prp2"] as System.Collections.IDictionary)["prp3"]);
        }

        [Test]
        public async Task ComposeSite_ContentMetadataArray()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"),
                "---\r\nprp1: A\r\nprp2:\r\n  - B\r\n  - C\r\n---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(2, site.MainPage.Data.Count);
            Assert.AreEqual("A", site.MainPage.Data["prp1"]);
            Assert.AreEqual(2, (site.MainPage.Data["prp2"] as IEnumerable<object>).Count());
            Assert.AreEqual("B", (site.MainPage.Data["prp2"] as IEnumerable<object>).ElementAt(0));
            Assert.AreEqual("C", (site.MainPage.Data["prp2"] as IEnumerable<object>).ElementAt(1));
        }
        
        [Test]
        public async Task ComposeSite_ContentNoFrontMatter()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"),
                "Text Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual("Text Line1\r\nText Line2", site.MainPage.RawContent);
            Assert.AreEqual(0, site.MainPage.Data.Count);
        }

        [Test]
        public async Task ComposeSite_NotClosedFrontMatter()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"),
                "---\r\nText Line1\r\nText Line2"),
            }.ToAsyncEnumerable();

            await AssertException.ThrowsInnerAsync<FrontMatterErrorException>(() => m_Composer.ComposeSite(src, ""));
        }
    }
}
