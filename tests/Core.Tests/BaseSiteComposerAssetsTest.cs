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
using System.Threading.Tasks;
using Tests.Common.Mocks;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Core.Tests
{
    public class BaseSiteComposerAssetsTest
    {
        private BaseSiteComposer m_Composer;

        [SetUp]
        public void Setup() 
        {
            m_Composer = new BaseSiteComposer(new Mock<ILayoutParser>().Object, null, 
                new Mock<IComposerExtension>().Object);
        }

        [Test]
        public async Task ComposeSite_SinglePageAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1\asset.txt"), "a1"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(0, site.MainPage.Assets.Count);
            Assert.AreEqual(1, site.MainPage.SubPages[0].Assets.Count);
            Assert.AreEqual("a1", site.MainPage.SubPages[0].Assets[0].AsTextContent());
        }

        [Test]
        public async Task ComposeSite_MainPageAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"asset.txt"), "a1"),
            }.ToAsyncEnumerable();
            
            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual("a1", site.MainPage.Assets[0].AsTextContent());
        }

        [Test]
        public async Task ComposeSite_TextAndBinaryAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1\asset.txt"), "a1"),
                new FileMock(Location.FromPath(@"page1\asset1.bin"), new byte[] { 1,2,3 })
            }.ToAsyncEnumerable();
            
            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(0, site.MainPage.Assets.Count);
            Assert.AreEqual(2, site.MainPage.SubPages[0].Assets.Count);
            Assert.AreEqual("a1", site.MainPage.SubPages[0].Assets.Find(a => a.FileName == "asset.txt").AsTextContent());
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual(site.MainPage.SubPages[0].Assets.Find(a => a.FileName == "asset1.bin").Content));
        }

        [Test]
        public async Task ComposeSite_MultiLevelAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"asset.txt"), "a1"),
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1\asset1.txt"), "a2"),
                new FileMock(Location.FromPath(@"page2\index.md"), ""),
                new FileMock(Location.FromPath(@"page2\asset.txt"), "a3"),
                new FileMock(Location.FromPath(@"page2\page3\index.md"), ""),
                new FileMock(Location.FromPath(@"page2\page3\asset2.txt"), "a4")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual("a1", site.MainPage.Assets[0].AsTextContent());
            Assert.AreEqual("asset.txt", site.MainPage.Assets[0].FileName);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Name == "page1").Assets.Count);
            Assert.AreEqual("a2", site.MainPage.SubPages.Find(p => p.Name == "page1").Assets[0].AsTextContent());
            Assert.AreEqual("asset1.txt", site.MainPage.SubPages.Find(p => p.Name == "page1").Assets[0].FileName);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Name == "page2").Assets.Count);
            Assert.AreEqual("a3", site.MainPage.SubPages.Find(p => p.Name == "page2").Assets[0].AsTextContent());
            Assert.AreEqual("asset.txt", site.MainPage.SubPages.Find(p => p.Name == "page2").Assets[0].FileName);
            Assert.AreEqual(1, site.MainPage.SubPages.Find(p => p.Name == "page2").SubPages[0].Assets.Count);
            Assert.AreEqual("a4", site.MainPage.SubPages.Find(p => p.Name == "page2").SubPages[0].Assets[0].AsTextContent());
            Assert.AreEqual("asset2.txt", site.MainPage.SubPages.Find(p => p.Name == "page2").SubPages[0].Assets[0].FileName);
        }

        [Test]
        public async Task ComposeSite_NoExtensionAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"asset"), "a1"),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual("asset", site.MainPage.Assets[0].FileName);
            Assert.AreEqual("a1", site.MainPage.Assets[0].AsTextContent());
        }

        [Test]
        public async Task ComposeSite_SubFolderAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1\sub-folder\asset1.txt"), "a1"),
                new FileMock(Location.FromPath(@"page1\sub-folder\sub-folder2\asset2.txt"), "a2"),
                new FileMock(Location.FromPath(@"page1\asset3.txt"), "a3")
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Name == "page1");

            var f1 = p1.Folders.First(f => f.Name == "sub-folder");
            var f2 = f1.Folders.First(f => f.Name == "sub-folder2");

            var a1 = f1.Assets.FirstOrDefault(a => a.FileName == "asset1.txt");
            var a2 = f2.Assets.FirstOrDefault(a => a.FileName == "asset2.txt");
            var a3 = p1.Assets.FirstOrDefault(a => a.FileName == "asset3.txt");

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
        public async Task ComposeSite_PhantomPageAsset()
        {
            var src = new FileMock[]
            {
                new FileMock(Location.FromPath(@"index.md"), ""),
                new FileMock(Location.FromPath(@"page1\index.md"), ""),
                new FileMock(Location.FromPath(@"page1\page2\asset1.txt"), "a1"),
                new FileMock(Location.FromPath(@"page1\page2\Page3\asset2.txt"), "a2"),
                new FileMock(Location.FromPath(@"page1\page2\Page3\index.md"), ""),
            }.ToAsyncEnumerable();

            var site = await m_Composer.ComposeSite(src, "");

            var p1 = site.MainPage.SubPages.First(p => p.Name == "page1");
            var p2 = p1.SubPages.First(p => p.Name == "page2");
            var p3 = p2.SubPages.First(p => p.Name == "Page3");

            var a1 = p1.Folders.FirstOrDefault(f => f.Name == "page2").Assets.FirstOrDefault(a => a.FileName == "asset1.txt");
            var a2 = p3.Assets.FirstOrDefault(a => a.FileName == "asset2.txt");

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

        //TODO: thsi is throwing error - investigate
        //[Test]
        //public async Task ComposeSite_NonDefaultPageAsset()
        //{
        //    var src = new FileMock[]
        //    {
        //        new FileMock(Location.FromPath(@"index.md"), ""),
        //        new FileMock(Location.FromPath(@"page1\page2.md"), ""),
        //        new FileMock(Location.FromPath(@"page1\page2\asset1.txt"), "a1"),
        //        new FileMock(Location.FromPath(@"page3\index.md"), ""),//commenting this line - works OK
        //        //new FileMock(Location.FromPath(@"page3\page4.md"), ""),
        //        //new FileMock(Location.FromPath(@"page3\page4\asset2.txt"), "a2")
        //    }.ToAsyncEnumerable();

        //    var site = await m_Composer.ComposeSite(src, "");
        //}
    }
}
