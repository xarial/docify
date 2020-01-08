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
    }
}
