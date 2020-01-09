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
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup()
        {
            m_Compiler = new DocifyEngine("", "").Resove<ICompiler>();
        }
        
        [Test]
        public async Task NoTemplatePageTest()
        {
            var p1 = new Page(new Location("page1.html"),
                "<div>@Model.Site.MainPage.Children.Count <a href=\"@Model.Page.Location.FileName\">Test</a></div>");

            var p2 = new Page(new Location("page2.html"),
                "<p>@Model.Site.MainPage.Children.Count</p>\n\n[site](https://www.mysite.com/page2.html)");

            p1.Children.Add(p2);

            var site = new Site("", p1);

            await m_Compiler.Compile(site);

            Assert.AreEqual("<div>1 <a href=\"page1.html\">Test</a></div>", site.MainPage.Content);
            Assert.AreEqual("<p>1</p>\n<p><a href=\"https://www.mysite.com/page2.html\">site</a></p>", site.MainPage.Children.FirstOrDefault(p => p.Key == "page2.html").Content);
        }
        
        [Test]
        public async Task TemplatePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "My Page Content",
                new Template("t1", "TemplateText1{{ content }}TemplateText2")));
            
            await m_Compiler.Compile(site);

            Assert.AreEqual("<p>TemplateText1<p>My Page Content</p>TemplateText2</p>", site.MainPage.Content);
        }
        
        [Test]
        public async Task NestedTemplatePageTest()
        {
            var t2 = new Template("t2", "*T2* @Model.Page.Location.FileName {{ content }}_T2");
            var t1 = new Template("t1", "*T1* @Model.Page.Location.FileName {{ content }}_T1", null, t2);
            var t3 = new Template("t3", "T3{{ content }}T3");
            var t4 = new Template("t4", "T4{{ content }}T4", null, t3);

            var p1 = new Page(new Location("page1.html"), "**Page1** @Model.Page.Location.FileName", t1);
            var p2 = new Page(new Location("page2.html"), "**Page2** @Model.Page.Location.FileName", t1);
            var p3 = new Page(new Location("page3.html"), "Page3", t4);
            var p4 = new Page(new Location("page4.html"), "Page4", t4);

            p1.Children.Add(p2);
            p1.Children.Add(p3);
            p1.Children.Add(p4);

            var site = new Site("", p1);

            await m_Compiler.Compile(site);

            Assert.AreEqual("<p><em>T2</em> page1.html <p><em>T1</em> page1.html <p><strong>Page1</strong> page1.html</p>_T1</p>_T2</p>", site.MainPage.Content);
            Assert.AreEqual("<p><em>T2</em> page2.html <p><em>T1</em> page2.html <p><strong>Page2</strong> page2.html</p>_T1</p>_T2</p>", site.MainPage.Children.FirstOrDefault(p => p.Key == "page2.html").Content);
            Assert.AreEqual("<p>T3<p>T4<p>Page3</p>T4</p>T3</p>", site.MainPage.Children.FirstOrDefault(p => p.Key == "page3.html").Content);
            Assert.AreEqual("<p>T3<p>T4<p>Page4</p>T4</p>T3</p>", site.MainPage.Children.FirstOrDefault(p => p.Key == "page4.html").Content);
        }
    }
}