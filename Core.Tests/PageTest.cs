using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Core.Tests
{
    public class PageTest
    {
        [Test]
        public void FindPageTest() 
        {
            var p1 = new Page(Location.FromPath("index.html"), "");
            var p2 = new Page(Location.FromPath(@"page2\index.html"), "");
            var p3 = new Page(Location.FromPath(@"page3\index.html"), "");
            var p4 = new Page(Location.FromPath(@"page4.html"), "");
            var p5 = new Page(Location.FromPath(@"page3\page5\index.html"), "");
            var p6 = new Page(Location.FromPath(@"page3\page5\page6\index.html"), "");

            p1.SubPages.Add(p2);
            p1.SubPages.Add(p3);
            p1.SubPages.Add(p4);
            p3.SubPages.Add(p5);
            p5.SubPages.Add(p6);

            var r1 = p1.FindPage(Location.FromPath(@"page3\index.html"), false);
            var r2 = p3.FindPage(Location.FromPath(@"page5\index.html"), true);
            var r3 = p1.FindPage(Location.FromPath(@"page4.html"), true);
            var r4 = p1.FindPage(Location.FromPath(@"page3\page5\page6\index.html"), true);

            Assert.IsNotNull(r1);
            Assert.IsNotNull(r2);
            Assert.IsNotNull(r3);
            Assert.IsNotNull(r4);
            Assert.AreEqual("page3::index.html", r1.Location.ToId());
            Assert.AreEqual("page3::page5::index.html", r2.Location.ToId());
            Assert.AreEqual("page4.html", r3.Location.ToId());
            Assert.AreEqual("page3::page5::page6::index.html", r4.Location.ToId());
            Assert.Throws<Exception>(() => p1.FindPage(Location.FromPath(@"page6\index.html"), false));
        }

        [Test]
        public void FindAssetTest() 
        {
            var p1 = new Page(Location.FromPath("index.html"), "");
            var p2 = new Page(Location.FromPath(@"page2\index.html"), "");
            var p3 = new Page(Location.FromPath(@"page3\index.html"), "");
            var p4 = new Page(Location.FromPath(@"page4.html"), "");
            var p5 = new PhantomPage(Location.FromPath(@"page3\page5\index.html"));
            var p6 = new Page(Location.FromPath(@"page3\page5\page6\index.html"), "");

            p1.SubPages.Add(p2);
            p1.SubPages.Add(p3);
            p1.SubPages.Add(p4);
            p3.SubPages.Add(p5);
            p5.SubPages.Add(p6);

            p3.Assets.Add(new File(Location.FromPath(@"page3\a1.txt"), "a1"));
            p6.Assets.Add(new File(Location.FromPath(@"page3\page5\page6\a2.txt"), "a2"));
            p3.Assets.Add(new File(Location.FromPath(@"page3\page5\a3.txt"), "a3"));

            var r1 = p1.FindAsset(Location.FromPath(@"page3\a1.txt"));
            var r2 = p3.FindAsset(Location.FromPath(@"a1.txt"));
            var r3 = p6.FindAsset(Location.FromPath(@"a2.txt"));
            var r4 = p6.FindAsset(Location.FromPath(@"page3\page5\page6\a2.txt"), false);
            var r5 = p3.FindAsset(Location.FromPath(@"page3\page5\page6\a2.txt"), false);
            var r6 = p3.FindAsset(Location.FromPath(@"page5\page6\a2.txt"));
            var r7 = p3.FindAsset(Location.FromPath(@"page5\a3.txt"));

            Assert.AreEqual("a1", r1.AsTextContent());
            Assert.AreEqual("a1", r2.AsTextContent());
            Assert.AreEqual("a2", r3.AsTextContent());
            Assert.AreEqual("a2", r4.AsTextContent());
            Assert.AreEqual("a2", r5.AsTextContent());
            Assert.AreEqual("a2", r6.AsTextContent());
            Assert.AreEqual("a3", r7.AsTextContent());

            Assert.Throws<Exception>(() => p2.FindAsset(Location.FromPath(@"page3\page5\page6\a2.txt"), false));
        }

        [Test]
        public void FindAssetPhantomPage() 
        {
            var p1 = new Page(Location.FromPath("index.html"), "");
            var p2 = new Page(Location.FromPath(@"page2\index.html"), "");

            p1.Assets.Add(new File(Location.FromPath(@"assets\a1.txt"), "a1"));
            p2.Assets.Add(new File(Location.FromPath(@"page2\assets\a2.txt"), "a2"));

            var r1 = p1.FindAsset(Location.FromPath(@"assets\a1.txt"));
            var r2 = p2.FindAsset(Location.FromPath(@"assets\a2.txt"));

            Assert.AreEqual("a1", r1.AsTextContent());
            Assert.AreEqual("a2", r2.AsTextContent());
        }
    }
}
