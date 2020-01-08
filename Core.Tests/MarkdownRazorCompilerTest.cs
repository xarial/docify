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
            var site = new Site("https://www.mysite.com", null, new Page[]
            {
                new Page("https://www.mysite.com/page.html", 
                null, null,
                "<div>@Model.Site.Pages.Length <a href=\"@Model.Page.Url\">Test</a></div>")
            });

            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            await comp.Compile(site);

            Assert.AreEqual("<div>1 <a href=\"https://www.mysite.com/page.html\">Test</a></div>", site.Pages.ElementAt(0).Content);
        }

        [Test]
        public async Task Compile_MultipleNestedPagesTest()
        {
            var site = new Site("mysite.com", null, new Page[]
            {
                new Page("page1.html",
                    null, null, "<p>P1</p>", 
                    new List<Page>(new Page[]
                    {
                        new Page("page2.html", null, null, "<p>P2</p>"),
                        new Page("page3.html", null, null, "<p>P3</p>")
                    })),
                new Page("page4.html", null, null, "<p>P4</p>")
            });

            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            await comp.Compile(site);

            Assert.AreEqual("<p>P1</p>", site.Pages.First(p => p.Url == "page1.html").Content);
            Assert.AreEqual("<p>P4</p>", site.Pages.First(p => p.Url == "page4.html").Content);
            Assert.AreEqual("<p>P2</p>", site.Pages.First(p => p.Url == "page1.html").Children.First(p => p.Url == "page2.html").Content);
            Assert.AreEqual("<p>P3</p>", site.Pages.First(p => p.Url == "page1.html").Children.First(p => p.Url == "page3.html").Content);
        }

        [Test]
        public async Task Compile_SinglePageMarkdownTest()
        {
            var site = new Site("https://www.mysite.com", null, new Page[]
            {
                new Page("https://www.mysite.com/page.html",
                null, null,
                "<p>@Model.Site.Pages.Length</p>\n\n[site](https://www.mysite.com/page.html)")
            });

            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, new Mock<ILogger>().Object, null);
            await comp.Compile(site);

            Assert.AreEqual("<p>1</p>\n<p><a href=\"https://www.mysite.com/page.html\">site</a></p>", site.Pages.ElementAt(0).Content);
        }
    }
}