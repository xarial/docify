﻿using Components.Tests.Properties;
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

namespace Components.Tests
{
    public class SitemapTest
    {
        private const string INCLUDE_PATH = @"sitemap\_includes\sitemap.cshtml";

        [Test]
        public async Task BasicTest()
        {
            var xmlFilePath = ComponentsTest.GetPath(@"sitemap\sitemap.xml");

            var site = ComponentsTest.NewSite("", INCLUDE_PATH);
            var p1 = new Page("Page1", "", ComponentsTest.GetData<Metadata>("title: p1"));
            var p2 = new Page("Page2", "", ComponentsTest.GetData<Metadata>("title: p2"));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(p2);
            site.MainPage.Assets.Add(new Asset("sitemap.xml", System.IO.File.ReadAllBytes(xmlFilePath)));

            var compiler = new DocifyEngine("", "", "", Environment_e.Test).Resove<ICompiler>();
            var files = await compiler.Compile(site).ToListAsync();

            var sitemap = files.First(f => f.Location.FileName == "sitemap.xml");

            Assert.AreEqual(Resources.sitemap1, ComponentsTest.Normalize(sitemap.AsTextContent()));
        }
    }
}
