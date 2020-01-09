//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.CLI;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;

namespace CLI.Tests
{
    public class CompilerTest
    {
        //TODO: group tests
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup()
        {
            m_Compiler = new DocifyEngine("", "").Resove<ICompiler>();
        }
        
        [Test]
        public async Task SinglePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "<div>@Model.Site.MainPage.Children.Count <a href=\"@Model.Page.Location.FileName\">Test</a></div>"));

            await m_Compiler.Compile(site);

            Assert.AreEqual("<div>0 <a href=\"page.html\">Test</a></div>", site.MainPage.Content);
        }

        [Test]
        public async Task MultipleNestedPagesTest()
        {
            var p1 = new Page(new Location("page1.html"), "<p>P1</p>");

            var site = new Site("", p1);

            var p2 = new Page(new Location("page2.html"), "<p>P2</p>");
            p1.Children.Add(p2);
            p2.Children.Add(new Page(new Location("page3.html"), "<p>P3</p>"));
            var p4 = new Page(new Location("page4.html"), "<p>P4</p>");
            p2.Children.Add(p4);
            p4.Children.Add(new Page(new Location("page5.html"), "<p>P5</p>"));

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p>P1</p>", site.MainPage.Content);
            Assert.AreEqual("<p>P2</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Content);
            Assert.AreEqual("<p>P3</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Children.First(p => p.Location.ToId() == "page3.html").Content);
            Assert.AreEqual("<p>P4</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Children.First(p => p.Location.ToId() == "page4.html").Content);
            Assert.AreEqual("<p>P5</p>", site.MainPage.Children.First(p => p.Location.ToId() == "page2.html").Children.First(p => p.Location.ToId() == "page4.html").Children.First(p => p.Location.ToId() == "page5.html").Content);
        }

        [Test]
        public async Task SinglePageMarkdownTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "<p>@Model.Site.MainPage.Children.Count</p>\n\n[site](https://www.mysite.com/page.html)"));

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p>0</p>\n<p><a href=\"https://www.mysite.com/page.html\">site</a></p>", site.MainPage.Content);
        }

        [Test]
        public async Task SimpleTemplatePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "My Page Content",
                new Template("t1", "TemplateText1{{ content }}TemplateText2")));

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p>TemplateText1<p>My Page Content</p>TemplateText2</p>", site.MainPage.Content);
        }

        [Test]
        public async Task NestedSimpleTemplatePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "My Page Content",
                new Template("t1", "T1{{ content }}T1", null,
                new Template("t2", "T2{{ content }}T2"))));

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p>T2<p>T1<p>My Page Content</p>T1</p>T2</p>", site.MainPage.Content);
        }

        [Test]
        public async Task NestedTemplateMultiPageTest()
        {
            var t2 = new Template("t2", "*T2* @Model.Page.Location.FileName {{ content }}_T2");
            var t1 = new Template("t1", "*T1* @Model.Page.Location.FileName {{ content }}_T1", null, t2);

            var p1 = new Page(new Location("page1.html"), "**Page1** @Model.Page.Location.FileName", t1);

            p1.Children.Add(new Page(new Location("page2.html"), "**Page2** @Model.Page.Location.FileName", t1));

            var site = new Site("", p1);

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p><em>T2</em> page1.html <p><em>T1</em> page1.html <p><strong>Page1</strong> page1.html</p>_T1</p>_T2</p>", site.MainPage.Content);
            Assert.AreEqual("<p><em>T2</em> page2.html <p><em>T1</em> page2.html <p><strong>Page2</strong> page2.html</p>_T1</p>_T2</p>", site.MainPage.Children[0].Content);
        }

        [Test]
        public async Task NestedTemplateMarkdownMultiPageTest()
        {
            var t2 = new Template("t2", "T2{{ content }}T2");
            var t1 = new Template("t1", "T1{{ content }}T1", null, t2);

            var p1 = new Page(new Location("page.html"), "Page1", t1);

            p1.Children.Add(new Page(new Location("page2.html"), "Page2", t1));

            var site = new Site("", p1);

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p>T2<p>T1<p>Page1</p>T1</p>T2</p>", site.MainPage.Content);
            Assert.AreEqual("<p>T2<p>T1<p>Page2</p>T1</p>T2</p>", site.MainPage.Children[0].Content);
        }
    }
}