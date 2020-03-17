//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Components.Tests.Properties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Compiler.Context;

namespace Components.Tests
{
    public class TocTest
    {
        private const string INCLUDE_PATH = @"toc\_includes\toc.cshtml";

        [Test]
        public async Task DefinedMenuTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH,
                ComponentsTest.GetData<Metadata>("title: p1"),
                ComponentsTest.GetData<Configuration>("$toc:\r\n  menu:\r\n    - Page1:\r\n      - /p2.html\r\n      - SubPage2\r\n    - Page2"));

            site.MainPage.SubPages.Add(new Page(new Location("p2.html"), "", ComponentsTest.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc1, res);
        }

        [Test]
        public async Task AutoMenuTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc home_menu: false %}\r\n</div>", INCLUDE_PATH);
            var p1 = new Page(Location.FromPath("Page1.html"), "", ComponentsTest.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage1.html"), "", ComponentsTest.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage2.html"), "", ComponentsTest.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new Page(Location.FromPath("Page2.html"), "", ComponentsTest.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc2, res);
        }

        [Test]
        public async Task AutoMenuHomeDefaultTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH);
            var p1 = new Page(Location.FromPath("Page1.html"), "", ComponentsTest.GetData<Metadata>("title: p1"));
            site.MainPage.SubPages.Add(p1);

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc3, res);
        }

        [Test]
        public async Task CustomTitleTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH, null,
                ComponentsTest.GetData<Configuration>("$toc:\r\n  home_menu: false\r\n  title_attribute: abc"));
            var p1 = new Page(Location.FromPath("Page1.html"), "", ComponentsTest.GetData<Metadata>("title: p1\r\nabc: x1"));
            site.MainPage.SubPages.Add(p1);
            
            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc4, res);
        }

        [Test]
        public async Task RootPageTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc { home_menu: false, root_page: /page1.html } %}\r\n</div>", INCLUDE_PATH);
            var p1 = new Page(Location.FromPath("Page1.html"), "", ComponentsTest.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage1.html"), "", ComponentsTest.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage2.html"), "", ComponentsTest.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new Page(Location.FromPath("Page2.html"), "", ComponentsTest.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc5, res);
        }

        [Test]
        public void RootPageInvalidTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc { home_menu: false, root_page: /page1.html } %}\r\n</div>", INCLUDE_PATH);

            Assert.ThrowsAsync<NullReferenceException>(() => ComponentsTest.CompileMainPageNormalize(site));
        }

        [Test]
        public void OrderPagesTest() 
        {
            throw new NotImplementedException();
        }

        [Test]
        public void ExcludePageTest() 
        {
            throw new NotImplementedException();
        }

        [Test]
        public void CustomHomeMenuTitleTest()
        {
            //home_menu_title: custom
            throw new NotImplementedException();
        }
    }
}
