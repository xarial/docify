﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Components.Tests.Properties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.CLI;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Data;

namespace Components.Tests
{
    public class GoogleAnalyticsTest
    {
        private const string INCLUDE_PATH = @"google-analytics\_includes\google-analytics.cshtml";

        [Test]
        public async Task DefaultParamsTestEnvTest()
        {
            var site = ComponentsTest.Instance.NewSite("<head>\r\n{% google-analytics { traking-code:  } %}\r\n</head>", INCLUDE_PATH,
                null,
                new Configuration { Environment = "Test" });

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual("<head>\r\n</head>", res);
        }

        [Test]
        public async Task ProdEnvTest()
        {
            var site = ComponentsTest.Instance.NewSite("<head>\r\n{% google-analytics { traking-code: ABC } %}\r\n</head>", INCLUDE_PATH,
                null,
                new Configuration { Environment = "Production" });

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual($"<head>\r\n{Resources.google_analytics1}\r\n</head>", res);
        }

        [Test]
        public async Task TestEnvIgnoreEnvTest()
        {
            var site = ComponentsTest.Instance.NewSite("<head>\r\n{% google-analytics { environment: -, traking-code: ABC } %}\r\n</head>", INCLUDE_PATH,
                null,
                new Configuration { Environment = "Test" });

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);

            Assert.AreEqual($"<head>\r\n{Resources.google_analytics1}\r\n</head>", res);
        }

        [Test]
        public async Task NoAnalyticsIdTest()
        {
            var site = ComponentsTest.Instance.NewSite("<head>\r\n{% google-analytics %}\r\n</head>", INCLUDE_PATH,
                null,
                new Configuration { Environment = "Production" });

            var res = await ComponentsTest.Instance.CompileMainPageNormalize(site);
            
            Assert.AreEqual("<head>\r\n</head>", res);
        }
    }
}
