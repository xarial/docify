//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Core.Tests.Properties;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;

namespace Core.Tests
{
    public class MarkdownRazorCompilerTest
    {
        [Test]
        public async Task Compile_SinglePageTest()
        {
            var site = new Site("https://www.mysite.com",
                new Page(new Location("page.html"),
                "<div>@Model.Site.MainPage.Children.Count <a href=\"@Model.Page.Location.FileName\">Test</a></div>"));

            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            await comp.Compile(site);

            Assert.AreEqual("<div>0 <a href=\"page.html\">Test</a></div>", site.MainPage.Content);
        }

        [Test]
        public async Task Compile_MultipleNestedPagesTest()
        {
            var p1 = new Page(new Location("page1.html"), "<p>P1</p>");

            var site = new Site("mysite.com", p1);

            var p2 = new Page(new Location("page2.html"), "<p>P2</p>");
            p1.Children.Add(p2);
            p2.Children.Add(new Page(new Location("page3.html"), "<p>P3</p>"));
            var p4 = new Page(new Location("page4.html"), "<p>P4</p>");
            p2.Children.Add(p4);
            p4.Children.Add(new Page(new Location("page5.html"), "<p>P5</p>"));

            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            await comp.Compile(site);

            Assert.AreEqual("<p>P1</p>", site.MainPage.Content);
            Assert.AreEqual("<p>P2</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Content);
            Assert.AreEqual("<p>P3</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Children.First(p => p.Location.ToId() == "page3.html").Content);
            Assert.AreEqual("<p>P4</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Children.First(p => p.Location.ToId() == "page4.html").Content);
            Assert.AreEqual("<p>P5</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Children.First(p => p.Location.ToId() == "page4.html").Children.First(p => p.Location.ToId() == "page5.html").Content);
        }

        [Test]
        public async Task Compile_SinglePageMarkdownTest()
        {
            var site = new Site("https://www.mysite.com",
                new Page(new Location("page.html"),
                "<p>@Model.Site.MainPage.Children.Count</p>\n\n[site](https://www.mysite.com/page.html)"));
            
            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            await comp.Compile(site);

            Assert.AreEqual("<p>0</p>\n<p><a href=\"https://www.mysite.com/page.html\">site</a></p>", site.MainPage.Content);
        }
    }
}