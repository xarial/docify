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
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Fragments.Tests
{
    public class GoogleAnalyticsTest
    {
        private Task<string> Insert(Environment_e env, Metadata param) 
        {
            var p1 = new Page(Location.FromPath("index.html"), "");
            
            var site = new Site("www.example.com", null, new Configuration() { Environment = env });

            return FragmentTest.RenderIncludeNormalize(@"google-analytics\_includes\google-analytics.cshtml", param, site, p1);
        }

        [Test]
        public async Task DefaultParamsTestEnvTest()
        {
            var res = await Insert(Environment_e.Test, new Metadata() { { "traking_code", "" } });

            Assert.IsEmpty(res);
        }

        [Test]
        public async Task ProdEnvTest()
        {
            var res = await Insert(Environment_e.Production, new Metadata() { { "traking_code", "ABC" } });

            Assert.AreEqual(Resources.google_analytics1, res);
        }

        [Test]
        public async Task TestEnvIgnoreEnvTest()
        {
            var res = await Insert(Environment_e.Test, new Metadata() { { "production_only", "false" }, { "traking_code", "ABC" } });

            Assert.AreEqual(Resources.google_analytics1, res);
        }

        [Test]
        public void NoAnalyticsIdTest()
        {
            Assert.ThrowsAsync<NullReferenceException>(() => Insert(Environment_e.Production, null));
        }
    }
}
