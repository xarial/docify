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
using System.Linq;
using Moq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Composer;

namespace Core.Tests
{
    public class BaseSiteComposerAssetsTest
    {
        private BaseSiteComposer m_Composer;

        [SetUp]
        public void Setup() 
        {
            m_Composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null);
        }

        [Test]
        public void ComposeSite_SinglePageAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1\asset.txt"), "a1"),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(0, site.MainPage.Assets.Count);
            Assert.AreEqual(1, site.MainPage.SubPages[0].Assets.Count);
            Assert.AreEqual("a1", site.MainPage.SubPages[0].Assets[0].AsTextContent());
        }

        [Test]
        public void ComposeSite_MainPageAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"asset.txt"), "a1"),
            };
            
            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual("a1", site.MainPage.Assets[0].AsTextContent());
        }

        [Test]
        public void ComposeSite_TextAndBinaryAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1\asset.txt"), "a1"),
                new File(Location.FromPath(@"page1\asset1.bin"), new byte[] { 1,2,3 })
            };
            
            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(0, site.MainPage.Assets.Count);
            Assert.AreEqual(2, site.MainPage.SubPages[0].Assets.Count);
            Assert.AreEqual("a1", site.MainPage.SubPages[0].Assets.Find(a => a.Location.FileName == "asset.txt").AsTextContent());
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(site.MainPage.SubPages[0].Assets.Find(a => a.Location.FileName == "asset1.bin").Content));
        }

        [Test]
        public void ComposeSite_MultiLevelAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"asset.txt"), "a1"),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1\asset1.txt"), "a2"),
                new File(Location.FromPath(@"page2\index.md"), ""),
                new File(Location.FromPath(@"page2\asset.txt"), "a3"),
                new File(Location.FromPath(@"page2\page3\index.md"), ""),
                new File(Location.FromPath(@"page2\page3\asset2.txt"), "a4")
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual("a1", site.MainPage.Assets[0].AsTextContent());
            Assert.AreEqual("asset.txt", site.MainPage.Assets[0].Location.FileName);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Location.ToId() == "page1::index.html").Assets.Count);
            Assert.AreEqual("a2", site.MainPage.SubPages.Find(p => p.Location.ToId() == "page1::index.html").Assets[0].AsTextContent());
            Assert.AreEqual("asset1.txt", site.MainPage.SubPages.Find(p => p.Location.ToId() == "page1::index.html").Assets[0].Location.FileName);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Location.ToId() == "page2::index.html").Assets.Count);
            Assert.AreEqual("a3", site.MainPage.SubPages.Find(p => p.Location.ToId() == "page2::index.html").Assets[0].AsTextContent());
            Assert.AreEqual("asset.txt", site.MainPage.SubPages.Find(p => p.Location.ToId() == "page2::index.html").Assets[0].Location.FileName);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Location.ToId() == "page2::index.html").SubPages[0].Assets.Count);
            Assert.AreEqual("a4", site.MainPage.SubPages.Find(p => p.Location.ToId() == "page2::index.html").SubPages[0].Assets[0].AsTextContent());
            Assert.AreEqual("asset2.txt", site.MainPage.SubPages.Find(p => p.Location.ToId() == "page2::index.html").SubPages[0].Assets[0].Location.FileName);
        }

        [Test]
        public void ComposeSite_SubFolderAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1\sub-folder\asset1.txt"), "a1"),
            };

            var site = m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Location.ToId() == "page1::index.html");

            var a1 = p1.Assets.FirstOrDefault(a => a.Location.ToId() == "page1::sub-folder::asset1.txt");

            Assert.AreEqual(1, p1.Assets.Count);
            Assert.IsNotNull(a1);
            Assert.AreEqual("a1", a1.AsTextContent());
        }
    }
}
