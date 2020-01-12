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
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Components.Tests
{
    public class PageLangTest
    {
        private const string INCLUDE_PATH = @"page-lang\_includes\page-lang.cshtml";

        [Test]
        public async Task Default_Lang()
        {
            var site = ComponentsTest.NewSite("<html lang=\"{% page-lang %}\"/>", INCLUDE_PATH);

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual("<html lang=\"en\"/>", res);
        }

        [Test]
        public async Task Page_Lang()
        {
            var site = ComponentsTest.NewSite("<html lang=\"{% page-lang %}\"/>", INCLUDE_PATH, ComponentsTest.GetData<Metadata>("lang: ru"));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual("<html lang=\"ru\"/>", res);
        }

        [Test]
        public async Task Page_SiteDefLang()
        {
            var site = ComponentsTest.NewSite("<html lang=\"{% page-lang %}\"/>", INCLUDE_PATH, null, ComponentsTest.GetData<Configuration>("page-lang:\r\n  default_lang: fr"));

            var res = await ComponentsTest.CompileMainPageNormalize(site);

            Assert.AreEqual("<html lang=\"fr\"/>", res);
        }
    }
}
