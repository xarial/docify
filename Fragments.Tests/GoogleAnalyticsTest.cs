//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Fragments.Tests.Properties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.CLI;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Fragments.Tests
{
    public class GoogleAnalyticsTest
    {
        private const string INCLUDE_PATH = @"google-analytics\_includes\google-analytics.cshtml";

        private Task<string> Render(Environment_e env, Metadata param) 
        {
            var site = FragmentTest.NewSite(null, new Configuration { Environment = env });

            return FragmentTest.RenderIncludeNormalize(INCLUDE_PATH,
                param, site, site.MainPage);
        }

        private Task<string> Transform(Environment_e env, string content)
        {
            var site = FragmentTest.NewSite(null, new Configuration { Environment = env });

            return FragmentTest.TransformContentNormalize(INCLUDE_PATH, content, site, site.MainPage);
        }

        [Test]
        public async Task DefaultParamsTestEnvTest()
        {
            var res = await Render(Environment_e.Test, new Metadata() { { "traking_code", "" } });

            Assert.IsEmpty(res);
        }

        [Test]
        public async Task ProdEnvTest()
        {
            var res = await Render(Environment_e.Production, new Metadata() { { "traking_code", "ABC" } });

            Assert.AreEqual(Resources.google_analytics1, res);
        }

        [Test]
        public async Task TestEnvIgnoreEnvTest()
        {
            var res = await Render(Environment_e.Test, new Metadata() { { "production_only", "false" }, { "traking_code", "ABC" } });

            Assert.AreEqual(Resources.google_analytics1, res);
        }

        [Test]
        public async Task NoAnalyticsIdTest()
        {
            var res = await Render(Environment_e.Production, null);

            Assert.IsEmpty(res);
        }

        [Test]
        public async Task ProdEnvTransformTest()
        {
            var res = await Transform(Environment_e.Production, "{% google-analytics { traking_code: ABC } %}");
                       
            Assert.AreEqual(Resources.google_analytics1, res);
        }
    }
}
