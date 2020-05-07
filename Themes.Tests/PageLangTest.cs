using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;

namespace Themes.Tests
{
    public class PageLangTest
    {
        private const string INCLUDE_PATH = @"base\_includes\page-lang.cshtml";

        [Test]
        public async Task Default_Lang()
        {
            var site = ThemesTest.NewSite("<html lang=\"{% page-lang %}\"/>", INCLUDE_PATH);

            var res = await ThemesTest.CompileMainPageNormalize(site);

            Assert.AreEqual("<html lang=\"en\"/>", res);
        }

        [Test]
        public async Task Page_Lang()
        {
            var site = ThemesTest.NewSite("<html lang=\"{% page-lang %}\"/>", INCLUDE_PATH, ThemesTest.GetData<Metadata>("lang: ru"));

            var res = await ThemesTest.CompileMainPageNormalize(site);

            Assert.AreEqual("<html lang=\"ru\"/>", res);
        }

        [Test]
        public async Task Page_SiteDefLang()
        {
            var site = ThemesTest.NewSite("<html lang=\"{% page-lang %}\"/>", INCLUDE_PATH, null, ThemesTest.GetData<Configuration>("$page-lang:\r\n  default_lang: fr"));

            var res = await ThemesTest.CompileMainPageNormalize(site);

            Assert.AreEqual("<html lang=\"fr\"/>", res);
        }
    }
}
