﻿//*********************************************************************
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
using System.Linq;

namespace Core.Tests
{
    public class SiteComposerAssetsTest
    {
        private SiteComposer NewComposer()
        {
            return new SiteComposer(new LayoutParser());
        }

        [Test]
        public void ComposeSite_SinglePageAsset()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page1\index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page1\asset.txt"), "a1"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.Assets.Count);
            Assert.AreEqual(1, site.MainPage.Children[0].Assets.Count);
            Assert.AreEqual("a1", (site.MainPage.Children[0].Assets[0] as TextAsset).RawContent);
        }

        [Test]
        public void ComposeSite_MainPageAsset()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
                new TextSourceFile(Location.FromPath(@"asset.txt"), "a1"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.Assets.Count);
            Assert.AreEqual(1, site.MainPage.Assets.Count);
            Assert.AreEqual("a1", (site.MainPage.Assets[0] as TextAsset).RawContent);
        }

        [Test]
        public void ComposeSite_TextAndBinaryAsset()
        {
            var src = new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page1\index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page1\asset.txt"), "a1"),
                new BinarySourceFile(Location.FromPath(@"page1\asset1.bin"), new byte[] { 1,2,3 })
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(2, site.Assets.Count);
            Assert.AreEqual(2, site.MainPage.Children[0].Assets.Count);
            Assert.AreEqual("a1", (site.MainPage.Children[0].Assets.Find(a => a.Location.FileName == "asset.txt") as TextAsset).RawContent);
            Assert.IsTrue(new byte[] { 1, 2, 3 }.SequenceEqual((site.MainPage.Children[0].Assets.Find(a => a.Location.FileName == "asset1.bin") as BinaryAsset).Content));
        }

        [Test]
        public void ComposeSite_MultiLevelAsset()
        {
            var src = new ISourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
                new TextSourceFile(Location.FromPath(@"asset.txt"), "a1"),
                new TextSourceFile(Location.FromPath(@"page1\index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page1\asset1.txt"), "a2"),
                new TextSourceFile(Location.FromPath(@"page2\index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page2\asset.txt"), "a3"),
                new TextSourceFile(Location.FromPath(@"page2\page3\index.md"), ""),
                new TextSourceFile(Location.FromPath(@"page2\page3\asset2.txt"), "a4"),
            };

            var composer = NewComposer();

            var site = composer.ComposeSite(src, "");

            Assert.AreEqual(4, site.Assets.Count);
            Assert.AreEqual(1, site.MainPage.Children[0].Assets.Count);
            Assert.AreEqual("a1", (site.MainPage.Assets[0] as TextAsset).RawContent);
            Assert.AreEqual("asset.txt", site.MainPage.Assets[0].Location.FileName);
            Assert.AreEqual(1, site.MainPage.Children.Find(p => p.Location.ToId() == "page1-index.html").Assets.Count);
            Assert.AreEqual("a2", (site.MainPage.Children.Find(p => p.Location.ToId() == "page1-index.html").Assets[0] as TextAsset).RawContent);
            Assert.AreEqual("asset1.txt", site.MainPage.Children.Find(p => p.Location.ToId() == "page1-index.html").Assets[0].Location.FileName);
            Assert.AreEqual(1, site.MainPage.Children.Find(p => p.Location.ToId() == "page2-index.html").Assets.Count);
            Assert.AreEqual("a3", (site.MainPage.Children.Find(p => p.Location.ToId() == "page2-index.html").Assets[0] as TextAsset).RawContent);
            Assert.AreEqual("asset.txt", site.MainPage.Children.Find(p => p.Location.ToId() == "page2-index.html").Assets[0].Location.FileName);

            Assert.AreEqual(1, site.MainPage.Children.Find(p => p.Location.ToId() == "page2-index.html").Children[0].Assets.Count);
            Assert.AreEqual("a4", (site.MainPage.Children.Find(p => p.Location.ToId() == "page2-index.html").Children[0].Assets[0] as TextAsset).RawContent);
            Assert.AreEqual("asset2.txt", site.MainPage.Children.Find(p => p.Location.ToId() == "page2-index.html").Children[0].Assets[0].Location.FileName);
        }
    }
}