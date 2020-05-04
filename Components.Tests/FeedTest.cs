//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
            var site = ComponentsTest.NewSite("<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var p1 = new Page(Location.FromPath("Page1.html"), "", ComponentsTest.GetData<Metadata>("title: p1\r\ndescription: desc1"));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage1.html"), "", ComponentsTest.GetData<Metadata>("title: sp1")));
            p1.SubPages.Add(new Page(Location.FromPath("SubPage2.html"), "", ComponentsTest.GetData<Metadata>("title: sp2")));
            site.MainPage.SubPages.Add(p1);
            site.MainPage.SubPages.Add(new Page(Location.FromPath("Page2.html"), "", ComponentsTest.GetData<Metadata>("title: p2")));

            var res = UpdateBuildDate(await ComponentsTest.CompileMainPageNormalize(site));

            Assert.AreEqual(Resources.feed1, res);
        }

        [Test]
        public async Task ImageTest()
        {
            var site1 = ComponentsTest.NewSite("<channel>\r\n{% feed image: /img1.png %}\r\n</channel>", INCLUDE_PATH);
            var res1 = UpdateBuildDate(await ComponentsTest.CompileMainPageNormalize(site1));

            var site2 = ComponentsTest.NewSite("---\r\nimage: /img1.svg\r\n\r\nimage-png: /img1.png\r\n---\r\n<channel>\r\n{% feed image: /img1.png %}\r\n</channel>", INCLUDE_PATH);
            var res2 = UpdateBuildDate(await ComponentsTest.CompileMainPageNormalize(site1));

            var site3 = ComponentsTest.NewSite("---\r\nimage: /img1.png\r\n---\r\n<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var res3 = UpdateBuildDate(await ComponentsTest.CompileMainPageNormalize(site1));

            Assert.AreEqual(Resources.feed2, res1);
            Assert.AreEqual(Resources.feed2, res2);
            Assert.AreEqual(Resources.feed2, res3);
        }

        [Test]
        public async Task CategoriesTest()
        {
            var site = ComponentsTest.NewSite("<channel>\r\n{% feed %}\r\n</channel>", INCLUDE_PATH);
            var p1 = new Page(Location.FromPath("Page1.html"), "", ComponentsTest.GetData<Metadata>("title: p1\r\ncategories:\r\n  - cat1\r\n  - cat2"));
            site.MainPage.SubPages.Add(p1);

            var res = UpdateBuildDate(await ComponentsTest.CompileMainPageNormalize(site));

            Assert.AreEqual(Resources.feed3, res);
        }

        [Test]
        public async Task IgnorePagesTest()
        {
            //sitemap: false
            throw new NotImplementedException();
        }

        [Test]
        public async Task PubDateTest()
        {
            //date: yyyy-MM-dd
            throw new NotImplementedException();
        }
    }
}
