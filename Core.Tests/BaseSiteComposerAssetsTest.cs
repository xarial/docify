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
            Assert.AreEqual("a1", site.MainPage.SubPages[0].Assets.Find(a => a.Name == "asset.txt").AsTextContent());
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(site.MainPage.SubPages[0].Assets.Find(a => a.Name == "asset1.bin").Content));
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
            Assert.AreEqual("asset.txt", site.MainPage.Assets[0].Name);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Name == "page1").Assets.Count);
            Assert.AreEqual("a2", site.MainPage.SubPages.Find(p => p.Name == "page1").Assets[0].AsTextContent());
            Assert.AreEqual("asset1.txt", site.MainPage.SubPages.Find(p => p.Name == "page1").Assets[0].Name);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Name == "page2").Assets.Count);
            Assert.AreEqual("a3", site.MainPage.SubPages.Find(p => p.Name == "page2").Assets[0].AsTextContent());
            Assert.AreEqual("asset.txt", site.MainPage.SubPages.Find(p => p.Name == "page2").Assets[0].Name);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Name == "page2").SubPages[0].Assets.Count);
            Assert.AreEqual("a4", site.MainPage.SubPages.Find(p => p.Name == "page2").SubPages[0].Assets[0].AsTextContent());
            Assert.AreEqual("asset2.txt", site.MainPage.SubPages.Find(p => p.Name == "page2").SubPages[0].Assets[0].Name);
        }

        [Test]
        public void ComposeSite_SubFolderAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1\sub-folder\asset1.txt"), "a1"),
                new File(Location.FromPath(@"page1\sub-folder\sub-folder2\asset2.txt"), "a2"),
                new File(Location.FromPath(@"page1\asset3.txt"), "a3")
            };

            var site = m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Name == "page1");

            var f1 = p1.Folders.First(f => f.Name == "sub-folder");
            var f2 = f1.Folders.First(f => f.Name == "sub-folder2");

            var a1 = f1.Assets.FirstOrDefault(a => a.Name == "asset1.txt");
            var a2 = f2.Assets.FirstOrDefault(a => a.Name == "asset2.txt");
            var a3 = p1.Assets.FirstOrDefault(a => a.Name == "asset3.txt");

            Assert.AreEqual(1, p1.Assets.Count);
            Assert.AreEqual(1, f1.Assets.Count);
            Assert.AreEqual(1, f2.Assets.Count);
            Assert.AreEqual(1, p1.Folders.Count);
            Assert.AreEqual(1, f1.Folders.Count);
            Assert.AreEqual(0, f2.Folders.Count);
            Assert.IsNotNull(a1);
            Assert.IsNotNull(a2);
            Assert.IsNotNull(a3);
            Assert.AreEqual("a1", a1.AsTextContent());
            Assert.AreEqual("a2", a2.AsTextContent());
            Assert.AreEqual("a3", a3.AsTextContent());
        }

        [Test]
        public void ComposeSite_PhantomPageAsset()
        {
            var src = new File[]
            {
                new File(Location.FromPath(@"index.md"), ""),
                new File(Location.FromPath(@"page1\index.md"), ""),
                new File(Location.FromPath(@"page1\page2\asset1.txt"), "a1"),
                new File(Location.FromPath(@"page1\page2\Page3\asset2.txt"), "a2"),
                new File(Location.FromPath(@"page1\page2\Page3\index.md"), ""),
            };

            var site = m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Name == "page1");
            var p2 = p1.SubPages.First(p => p.Name == "page2");
            var p3 = p2.SubPages.First(p => p.Name == "Page3");

            var a1 = p1.Folders.FirstOrDefault(f => f.Name == "page2").Assets.FirstOrDefault(a => a.Name == "asset1.txt");
            var a2 = p3.Assets.FirstOrDefault(a => a.Name == "asset2.txt");

            Assert.AreEqual(0, p1.Assets.Count);
            Assert.AreEqual(1, p1.Folders.Count);
            Assert.AreEqual(1, p1.Folders[0].Assets.Count);
            Assert.AreEqual(0, p2.Assets.Count);
            Assert.AreEqual(1, p3.Assets.Count);
            Assert.IsNotNull(a1);
            Assert.IsNotNull(a2);
            Assert.AreEqual("a1", a1.AsTextContent());
            Assert.AreEqual("a2", a2.AsTextContent());
        }
    }
}
