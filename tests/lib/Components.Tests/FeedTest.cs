//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Components.Tests.Properties;
using NUnit.Framework;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using System.Text.RegularExpressions;
using System;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core;
using System.Xml;
using Xarial.Docify.CLI;
using Xarial.Docify.Base.Services;
using System.Linq;
using System.Xml.Schema;
using System.ServiceModel.Syndication;
using Tests.Common.Mocks;

namespace Components.Tests
{
    public class FeedTest
    {
        private const string INCLUDE_PATH = @"feed\_includes\feed.cshtml";

        private string UpdateBuildDate(string feed) 
        {
            return Regex.Replace(feed, "(?<=<lastBuildDate>).*(?=</lastBuildDate>)", "-");
        }

        [Test]
        public async Task DefaultTest()
        {
            var site = ComponentsTest.Instance.NewSite("<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1\r\ndescription: desc1"));
            p1.SubPages.Add(new PageMock("SubPage1", "", ComponentsTest.Instance.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new PageMock("SubPage2", "", ComponentsTest.Instance.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new PageMock("Page2", "", ComponentsTest.Instance.GetData<Metadata>("title: p2")));

            var res = UpdateBuildDate(await ComponentsTest.Instance.CompileMainPageNormalize(site));

            Assert.AreEqual(Resources.feed1, res);
        }

        [Test]
        public async Task ImageTest()
        {
            var site1 = ComponentsTest.Instance.NewSite("<channel>\r\n{% feed image: /img1.png %}\r\n</channel>", INCLUDE_PATH);
            var res1 = UpdateBuildDate(await ComponentsTest.Instance.CompileMainPageNormalize(site1));

            var site2 = ComponentsTest.Instance.NewSite("---\r\nimage: /img1.svg\r\n\r\nimage-png: /img1.png\r\n---\r\n<channel>\r\n{% feed image: /img1.png %}\r\n</channel>", INCLUDE_PATH);
            var res2 = UpdateBuildDate(await ComponentsTest.Instance.CompileMainPageNormalize(site1));

            var site3 = ComponentsTest.Instance.NewSite("---\r\nimage: /img1.png\r\n---\r\n<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var res3 = UpdateBuildDate(await ComponentsTest.Instance.CompileMainPageNormalize(site1));

            Assert.AreEqual(Resources.feed2, res1);
            Assert.AreEqual(Resources.feed2, res2);
            Assert.AreEqual(Resources.feed2, res3);
        }

        [Test]
        public async Task CategoriesTest()
        {
            var site = ComponentsTest.Instance.NewSite("<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1\r\ncategories:\r\n  - cat1\r\n  - cat2"));
            site.MainPage.SubPages.Add(p1);

            var res = UpdateBuildDate(await ComponentsTest.Instance.CompileMainPageNormalize(site));

            Assert.AreEqual(Resources.feed3, res);
        }

        [Test]
        public async Task IgnorePagesTest()
        {
            var site = ComponentsTest.Instance.NewSite("<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1\r\nsitemap: false"));
            site.MainPage.SubPages.Add(p1);

            var res = UpdateBuildDate(await ComponentsTest.Instance.CompileMainPageNormalize(site));

            Assert.AreEqual(Resources.feed4, res);
        }

        [Test]
        public async Task PubDateTest()
        {
            var xmlFilePath = ComponentsTest.Instance.GetPath(@"feed\feed.xml");

            var site = ComponentsTest.Instance.NewSite("", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1\r\ndate: 2020-05-06"));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.Assets.Add(new AssetMock("feed.xml", System.IO.File.ReadAllBytes(xmlFilePath)));

            var compiler = new DocifyEngineMock().Resove<ICompiler>();
            var files = await compiler.Compile(site).ToListAsync();

            var feed = files.First(f => f.Location.FileName == "feed.xml");

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(feed.AsTextContent());
            
            var node = xmlDoc.SelectSingleNode("//channel/item[title='p1']/pubDate").InnerText;
            Assert.AreEqual("Wed, 06 May 2020 00:00:00 GMT", node);
        }

        [Test]
        public async Task SchemaTest()
        {
            var xmlFilePath = ComponentsTest.Instance.GetPath(@"feed\feed.xml");

            var site = ComponentsTest.Instance.NewSite("", INCLUDE_PATH);
            var p1 = new PageMock("Page1", "", ComponentsTest.Instance.GetData<Metadata>("title: p1\r\ndate: 2020-05-06"));
            var p2 = new PageMock("Page2", "", ComponentsTest.Instance.GetData<Metadata>("title: p2\r\ncategories:\r\n  - cat1\r\n  - cat2"));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(p2);
            site.MainPage.Assets.Add(new AssetMock("feed.xml", System.IO.File.ReadAllBytes(xmlFilePath)));

            var compiler = new DocifyEngineMock().Resove<ICompiler>();
            var files = await compiler.Compile(site).ToListAsync();

            var feed = files.First(f => f.Location.FileName == "feed.xml");

            Assert.DoesNotThrow(() => SyndicationFeed.Load(XmlReader.Create(new System.IO.StringReader(feed.AsTextContent()))));
        }
    }
}
