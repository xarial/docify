﻿//*********************************************************************
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
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;

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
                ComponentsTest.GetData<Configuration>("$toc:\r\n  menu:\r\n    - Page1:\r\n      - /p2/\r\n      - SubPage2\r\n    - Page2"));

            site.MainPage.SubPages.Add(new PageMock("p2", "", ComponentsTest.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc1, res);
        }

        [Test]
        public async Task AutoMenuTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc home_menu: false %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new PageMock("Page2", "", ComponentsTest.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc2, res);
        }

        [Test]
        public async Task AutoMenuHomeDefaultTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.GetData<Metadata>("title: p1"));
            site.MainPage.SubPages.Add(p1);

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc3, res);
        }

        [Test]
        public async Task CustomTitleTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH, null,
                ComponentsTest.GetData<Configuration>("$toc:\r\n  home_menu: false\r\n  title_attribute: abc"));
            var p1 = new PageMock("Page1", "", ComponentsTest.GetData<Metadata>("title: p1\r\nabc: x1"));
            site.MainPage.SubPages.Add(p1);
            
            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc4, res);
        }

        [Test]
        public async Task RootPageTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc { home_menu: false, root_page: /page1/ } %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new PageMock("Page2", "", ComponentsTest.GetData<Metadata>("title: p2")));

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
        public async Task CustomHomeMenuTitleTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc home_menu_title: custom-title %}\r\n</div>", INCLUDE_PATH);
            
            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc6, res);
        }

        [Test]
        public async Task OrderPagesTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH);
            site.MainPage.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.GetData<Metadata>("title: sp1\r\norder: 2")));
            site.MainPage.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.GetData<Metadata>("title: sp2\r\norder: 1")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc7, res);
        }

        [Test]
        public async Task ExcludePageTest()
        {
            var site = ComponentsTest.NewSite("<div>\r\n{% toc %}\r\n</div>", INCLUDE_PATH);
            site.MainPage.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.GetData<Metadata>("title: sp1\r\ntoc: false")));
            site.MainPage.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.GetData<Metadata>("title: sp2")));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.toc8, res);
        }
    }
}