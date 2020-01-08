//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************/

using Core.Tests.Properties;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;

namespace Core.Tests
{
    public class Tests
    {
        [Test]
        public void ComposeSite_MultiLevelIndexOnlyTest()
        {
            var logger = new Mock<ILogger>();
            var publisher = new Mock<IPublisher>();

            var siteSrc = new SiteSource(@"C:\MySite", new IPageSource[]
            {
                new PageSource(@"C:\MySite\page1\index.md", "", null),
                new PageSource(@"page2\index.md", "", null),
                new PageSource(@"C:\MySite\page2\subpage1\index.md", "", null),
                new PageSource(@"page2\subpage1\subsubpage1\index.md", "", null),
                new PageSource(@"C:\MySite\page1\subpage2\index.md", "", null),
                new PageSource(@"\page3\subpage3\index.md", "", null),
                new PageSource(@"C:\MySite\page2\subpage1\subsubpage2\index.md", "", null),
            }, null, null, null);

            var config = new MarkdownRazorCompilerConfig("");
            var comp = new MarkdownRazorCompiler(config, logger.Object, publisher.Object);
            var site = comp.ComposeSite(siteSrc);
            
            Assert.AreEqual(3, site.Pages.Count());
            Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page1"));
            Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page2"));
            Assert.IsNotNull(site.Pages.FirstOrDefault(p => p.Url == "page3"));

            Assert.AreEqual(1, site.Pages.First(p => p.Url == "page1").Children.Count());
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page1").Children.FirstOrDefault(p => p.Url == "page1/subpage2"));

            Assert.AreEqual(1, site.Pages.First(p => p.Url == "page2").Children.Count());
            Assert.AreEqual(2, site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.Count());
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.FirstOrDefault(p=>p.Url == "page2/subpage1/subsubpage1"));
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page2").Children.First(p => p.Url == "page2/subpage1").Children.FirstOrDefault(p => p.Url == "page2/subpage1/subsubpage2"));

            Assert.AreEqual(1, site.Pages.First(p => p.Url == "page3").Children.Count());
            Assert.IsNotNull(site.Pages.First(p => p.Url == "page3").Children.FirstOrDefault(p => p.Url == "page3/subpage3"));
        }

        [Test]
        public async Task CompileTest()
        {
            var logger = new Mock<ILogger>();
            var publisher = new Mock<IPublisher>();
            var site = new SiteSource("", new IPageSource[] 
            {
                new PageSource("", Resources.Sample1, null)
            }, null, null, null);

            var config = new MarkdownRazorCompilerConfig("mysite");
            var comp = new MarkdownRazorCompiler(config, logger.Object, publisher.Object);
            await comp.Compile(site);
        }
    }
}