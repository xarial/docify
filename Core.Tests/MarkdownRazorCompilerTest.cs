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
            //var site = new Site("https://www.mysite.com");
            //site.Pages.Add(new Page("https://www.mysite.com/page.html",
            //    "<div>@Model.Site.Pages.Count <a href=\"@Model.Page.Url\">Test</a></div>"));

            //var config = new MarkdownRazorCompilerConfig("mysite");
            //var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            //await comp.Compile(site);

            //Assert.AreEqual("<div>1 <a href=\"https://www.mysite.com/page.html\">Test</a></div>", site.Pages.ElementAt(0).Content);
        }

        [Test]
        public async Task Compile_MultipleNestedPagesTest()
        {
            //var site = new Site("mysite.com");

            //var p1 = new Page(new Location("page1.html"), "<p>P1</p>");
            //p1.Children.Add(new Page(new Location("page2.html"), "<p>P2</p>"));
            //p1.Children.Add(new Page(new Location("page3.html"), "<p>P3</p>"));
            //site.Pages.Add(p1);
            //site.Pages.Add(new Page(new Location("page4.html"), "<p>P4</p>"));

            //var config = new MarkdownRazorCompilerConfig("mysite");
            //var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            //await comp.Compile(site);

            //Assert.AreEqual("<p>P1</p>", site.Pages.First(p => p.Url == "page1.html").Content);
            //Assert.AreEqual("<p>P4</p>", site.Pages.First(p => p.Url == "page4.html").Content);
            //Assert.AreEqual("<p>P2</p>", site.Pages.First(p => p.Url == "page1.html").Children.First(p => p.Url == "page2.html").Content);
            //Assert.AreEqual("<p>P3</p>", site.Pages.First(p => p.Url == "page1.html").Children.First(p => p.Url == "page3.html").Content);
        }

        [Test]
        public async Task Compile_SinglePageMarkdownTest()
        {
            //var site = new Site("https://www.mysite.com");

            //site.Pages.Add(new Page("https://www.mysite.com/page.html",
            //    "<p>@Model.Site.Pages.Count</p>\n\n[site](https://www.mysite.com/page.html)"));
            
            //var config = new MarkdownRazorCompilerConfig("mysite");
            //var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            //await comp.Compile(site);

            //Assert.AreEqual("<p>1</p>\n<p><a href=\"https://www.mysite.com/page.html\">site</a></p>", site.Pages.ElementAt(0).Content);
        }
    }
}