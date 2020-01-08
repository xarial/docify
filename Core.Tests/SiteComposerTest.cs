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
using System.Linq;
using System.Text;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;

namespace Core.Tests
{
    public class SiteComposerTest
    {
        [Test]
        public void ComposeSite_MultiLevelIndexOnlyTest()
        {
            var elems = new IElementSource[]
            {
                new ElementSource(@"C:\MySite\page1\index.md", "", ElementType_e.Page),
                new ElementSource(@"page2\index.md", "", ElementType_e.Page),
                new ElementSource(@"C:\MySite\page2\subpage1\index.md", "", ElementType_e.Page),
                new ElementSource(@"page2\subpage1\subsubpage1\index.md", "", ElementType_e.Page),
                new ElementSource(@"C:\MySite\page1\subpage2\index.md", "", ElementType_e.Page),
                new ElementSource(@"\page3\subpage3\index.md", "", ElementType_e.Page),
                new ElementSource(@"C:\MySite\page2\subpage1\subsubpage2\index.md", "", ElementType_e.Page),
            };
            
            var composer = new SiteComposer();
            var site = composer.ComposeSite(elems, @"C:\MySite", "");

            Assert.AreEqual(3, site.Pages.Count());
            Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page1"));
            Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page2"));
            Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page3"));

            Assert.AreEqual(1, site.Pages.First(p => p.Url == "page1").Children.Count());
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page1").Children.FirstOrDefault(p => p.Url == "page1/subpage2"));

            Assert.AreEqual(1, site.Pages.First(p => p.Url == "page2").Children.Count());
            Assert.AreEqual(2, site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.Count());
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.FirstOrDefault(p => p.Url == "page2/subpage1/subsubpage1"));
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.FirstOrDefault(p => p.Url == "page2/subpage1/subsubpage2"));

            Assert.AreEqual(1, site.Pages.First(p => p.Url == "page3").Children.Count());
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page3").Children.FirstOrDefault(p => p.Url == "page3/subpage3"));
        }
    }
}
