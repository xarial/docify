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
using System.Text;
using System.Threading.Tasks;

namespace Components.Tests
{
    public class DisqusTest
    {
        private const string INCLUDE_PATH = @"disqus\_includes\disqus.cshtml";

        [Test]
        public async Task BasicTest()
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% disqus short-name: test %}\r\n</div>", INCLUDE_PATH);
            
            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.disqus1, res);
        }

        [Test]
        public async Task NoCommentsTest()
        {
            var site1 = ComponentsTest.Instance.NewSite("<div>\r\n{% disqus { short-name: test } %}\r\n</div>", INCLUDE_PATH,
                new Xarial.Docify.Core.Data.Metadata() { { "comments", false } });
            var res1 = await ComponentsTest.Instance.CompileMainPageNormalize(site1);

            var site2 = ComponentsTest.Instance.NewSite("<div>\r\n{% disqus short-name: test %}\r\n</div>", INCLUDE_PATH,
                new Xarial.Docify.Core.Data.Metadata() { { "sitemap", false } });
            var res2 = await ComponentsTest.Instance.CompileMainPageNormalize(site2);

            var site3 = ComponentsTest.Instance.NewSite("<div>\r\n{% disqus %}\r\n</div>", INCLUDE_PATH);
            var res3 = await ComponentsTest.Instance.CompileMainPageNormalize(site2);

            Assert.AreEqual("<div>\r\n</div>", res1);
            Assert.AreEqual("<div>\r\n</div>", res2);
            Assert.AreEqual("<div>\r\n</div>", res3);
        }

        [Test]
        public async Task NotCountTest() 
        {
            var site = ComponentsTest.Instance.NewSite("<div>\r\n{% disqus { short-name: test, count: false } %}\r\n</div>", INCLUDE_PATH);

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual(Resources.disqus2, res);
        }
    }
}
