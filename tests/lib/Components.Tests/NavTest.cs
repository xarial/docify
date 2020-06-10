//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
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
using Xarial.Docify.Lib.Tools.Exceptions;

namespace Components.Tests
{
    public class NavTest
    {
        private const string INCLUDE_PATH = @"nav\_includes\nav.cshtml";

        [Test]
        public async Task DefinedMenuTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav %}\r\n</div>", INCLUDE_PATH,
                ComponentsTest.Instance.GetData<Metadata>("title: p1"),
                ComponentsTest.Instance.GetData<Configuration>("$nav:\r\n  menu:\r\n    - Page1:\r\n      - /p2/\r\n      - SubPage2\r\n    - Page2"));

            site.MainPage.SubPages.Add(new PageMock("p2", "", ComponentsTest.Instance.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.nav1, res);
        }

        [Test]
        public async Task AutoMenuTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav home-menu: false %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.Instance.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.Instance.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new PageMock("Page2", "", ComponentsTest.Instance.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.nav2, res);
        }

        [Test]
        public async Task AutoMenuHomeDefaultTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1"));
            site.MainPage.SubPages.Add(p1);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.nav3, res);
        }

        [Test]
        public async Task CustomTitleTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav %}\r\n</div>", INCLUDE_PATH, null,
                ComponentsTest.Instance.GetData<Configuration>("$nav:\r\n  home-menu: false\r\n  title-attribute: abc"));
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1\r\nabc: x1"));
            site.MainPage.SubPages.Add(p1);
            
            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.nav4, res);
        }

        [Test]
        public async Task RootPageTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav { home-menu: false, root-page: /page1/ } %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.Instance.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.Instance.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new PageMock("Page2", "", ComponentsTest.Instance.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.nav5, res);
        }

        [Test]
        public async Task FilterPageTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav { filter: [ '|*/SubPage1/' ] } %}\r\n</div>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1"));
            p1.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.Instance.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.Instance.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new PageMock("Page2", "", ComponentsTest.Instance.GetData<Metadata>("title: p2")));

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.nav6, res);
        }

        [Test]
        public async Task RootPageInvalidTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% nav { home-menu: false, root-page: /page1.html } %}\r\n</div>", INCLUDE_PATH);

            Exception innerEx = null;
            try
            {
                await ComponentsTest.Instance.CompileMainPageNormalize(site);
            }
            catch (Exception ex)
            {
                innerEx = ex.InnerException;
            }

            Assert.IsInstanceOf<RootPageNotFoundException>(innerEx);
        }

        [Test]
        public async Task CustomTitleAndUrlTest()
        {
            throw new NotImplementedException();
        }
    }
}
