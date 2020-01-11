//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Compiler.Context;

namespace Fragments.Tests
{
    public class NavTest
    {
        private const string INCLUDE_PATH = @"nav\_includes\nav.cshtml";
        
        [Test]
        public async Task DefinedMenuTest() 
        {
            var site = FragmentTest.NewSite(null, FragmentTest.GetData<Configuration>("nav:\r\n  menu:\r\n    - Page1:\r\n      - SubPage1\r\n      - SubPage2\r\n    - Page2"));
            var res = await FragmentTest.RenderIncludeNormalize(INCLUDE_PATH, null, site, site.MainPage);
        }

        [Test]
        public async Task AutoMenuTest()
        {
            var site = FragmentTest.NewSite();
            var p1 = new Page(Location.FromPath("Page1.html"), "");
            p1.SubPages.Add(new Page(Location.FromPath("SubPage1.html"), ""));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage2.html"), ""));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new Page(Location.FromPath("Page2.html"), ""));

            var res = await FragmentTest.RenderIncludeNormalize(INCLUDE_PATH, null, site, site.MainPage);
        }
    }
}
