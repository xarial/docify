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
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Components.Tests
{
    public class SeoTest
    {
        private Task<string> Insert(Metadata param)
        {
            var site = ComponentsTest.NewSite(
                ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: d1"),
                ComponentsTest.GetData<Configuration>("title: t1\r\ndescription: sd1"));
                        
            return ComponentsTest.RenderIncludeNormalize(@"seo\_includes\seo.cshtml", param, site, site.MainPage);
        }

        [Test]
        public async Task DefaultParamsTest()
        {
            var res = await Insert(null);

            Assert.AreEqual(Resources.seo1, res);
        }

        [Test]
        public async Task OgParamsTest()
        {
            var res = await Insert(
                ComponentsTest.GetData<Metadata>("og: true\r\ntwitter: false\r\nlinkedin: false"));
            
            Assert.AreEqual(Resources.seo2, res);
        }

        [Test]
        public async Task TwitterParamsTest()
        {
            var res = await Insert(
                ComponentsTest.GetData<Metadata>("og: false\r\ntwitter: true\r\nlinkedin: false"));

            Assert.AreEqual(Resources.seo3, res);
        }

        [Test]
        public async Task LiParamsTest()
        {
            var res = await Insert(
                ComponentsTest.GetData<Metadata>("og: false\r\ntwitter: false\r\nlinkedin: true"));

            Assert.AreEqual(Resources.seo4, res);
        }
    }
}
