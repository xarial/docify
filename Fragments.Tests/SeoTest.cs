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
    public class SeoTest
    {
        private Task<string> Insert(Metadata param)
        {            
            var p1 = new Page(Location.FromPath("index.html"), "");
            var p2 = new Page(Location.FromPath("p2\\index.html"), "");
            p2.Data["title"] = "p2";
            p1.SubPages.Add(p2);

            var site = new Site("www.example.com", null, new Configuration( Environment_e.Test)
            { { "title", "t1" }, { "description", "d1" } });
            
            return FragmentTest.InsertIncludeToPageNormalize(@"seo\_includes\seo.cshtml", param, site, p2);
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
                new Metadata() 
                {
                    { "og", true },
                    { "twitter", false },
                    { "linkedin", false }
                });
            
            Assert.AreEqual(Resources.seo2, res);
        }

        [Test]
        public async Task TwitterParamsTest()
        {
            var res = await Insert(
                new Metadata()
                {
                    { "og", false },
                    { "twitter", true },
                    { "linkedin", false }
                });

            Assert.AreEqual(Resources.seo3, res);
        }

        [Test]
        public async Task LiParamsTest()
        {
            var res = await Insert(
                new Metadata()
                {
                    { "og", false },
                    { "twitter", false },
                    { "linkedin", true }
                });

            Assert.AreEqual(Resources.seo4, res);
        }
    }
}
