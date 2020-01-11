//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Core.Tests.Properties;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.Context;

namespace Core.Tests
{
    public class BaseCompilerTest
    {
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup() 
        {
            var contTransMock = new Mock<IContentTransformer>();
            contTransMock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IContextModel>()))
                .Returns<string, string, IContextModel>((c, k, m) => Task.FromResult(
                    c.Replace("_FN_", (m as ContextModel).Page.Name)
                    .Replace("_CC_", (m as ContextModel).Site.MainPage.SubPages.Count().ToString())));

            var layoutMock = new Mock<ILayoutParser>();

            layoutMock.Setup(m => m.ContainsPlaceholder(It.IsAny<string>()))
                .Returns<string>(c => c.Contains("_C_"));

            layoutMock.Setup(m => m.InsertContent(It.IsAny<Template>(), It.IsAny<string>(), It.IsAny<IContextModel>()))
                .Returns<Template, string, IContextModel>((t, c, m) =>
                {
                    string r = c;
                    Template tt = t;

                    while (tt != null)
                    {
                        r = tt.RawContent.Replace("_C_", r)
                        .Replace("_FN_", (m as ContextModel).Page.Name)
                        .Replace("_CC_", (m as ContextModel).Site.MainPage.SubPages.Count().ToString());

                        tt = tt.Layout;
                    }

                    return Task.FromResult(r);
                });


            var includesHandlerMock = new Mock<IIncludesHandler>();
            includesHandlerMock.Setup(m => m.ReplaceAll(It.IsAny<string>(), It.IsAny<Site>(), It.IsAny<Page>()))
                .Returns<string, Site, Page>((c, s, p) => Task.FromResult(c));

            m_Compiler = new BaseCompiler(new BaseCompilerConfig(),
                new Mock<ILogger>().Object,
                layoutMock.Object,
                contTransMock.Object,
                includesHandlerMock.Object);
        }

        [Test]
        public async Task Compile_SinglePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "abc _CC_ _FN_ test"), null);

            await m_Compiler.Compile(site);

            Assert.AreEqual("abc 0 page.html test", site.MainPage.Content);
        }

        [Test]
        public async Task Compile_MultipleNestedPagesTest()
        {
            var p1 = new Page(new Location("page1.html"), "P1");

            var site = new Site("", p1, null);

            var p2 = new Page(new Location("page2.html"), "P2");
            p1.SubPages.Add(p2);
            p2.SubPages.Add(new Page(new Location("page3.html"), "P3"));
            var p4 = new Page(new Location("page4.html"), "P4");
            p2.SubPages.Add(p4);
            p4.SubPages.Add(new Page(new Location("page5.html"), "P5"));

            await m_Compiler.Compile(site);

            Assert.AreEqual("P1", site.MainPage.Content);
            Assert.AreEqual("P2", site.MainPage.SubPages.First(p => p.Location.ToId() == "page2.html").Content);
            Assert.AreEqual("P3", site.MainPage.SubPages.First(p => p.Location.ToId() == "page2.html").SubPages.First(p => p.Location.ToId() == "page3.html").Content);
            Assert.AreEqual("P4", site.MainPage.SubPages.First(p => p.Location.ToId() == "page2.html").SubPages.First(p => p.Location.ToId() == "page4.html").Content);
            Assert.AreEqual("P5", site.MainPage.SubPages.First(p => p.Location.ToId() == "page2.html").SubPages.First(p => p.Location.ToId() == "page4.html").SubPages.First(p => p.Location.ToId() == "page5.html").Content);
        }

        [Test]
        public async Task Compile_SimpleTemplatePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "My Page Content",
                new Template("t1", "TemplateText1 _C_ TemplateText2")), null);

            await m_Compiler.Compile(site);

            Assert.AreEqual("TemplateText1 My Page Content TemplateText2", site.MainPage.Content);
        }

        [Test]
        public async Task Compile_NestedSimpleTemplatePageTest()
        {
            var site = new Site("",
                new Page(new Location("page.html"),
                "My Page Content",
                new Template("t1", "T1 _C_ T1", null,
                new Template("t2", "T2 _C_ T2"))), null);

            await m_Compiler.Compile(site);

            Assert.AreEqual("T2 T1 My Page Content T1 T2", site.MainPage.Content);
        }

        [Test]
        public async Task Compile_NestedTemplateMultiPageTest()
        {
            var t2 = new Template("t2", "T2 _FN_ _C_ T2");
            var t1 = new Template("t1", "T1 _FN_ _C_ T1", null, t2);

            var p1 = new Page(new Location("page1.html"), "Page1 _FN_", t1);

            p1.SubPages.Add(new Page(new Location("page2.html"), "Page2 _FN_", t1));

            var site = new Site("", p1, null);

            await m_Compiler.Compile(site);

            Assert.AreEqual("T2 page1.html T1 page1.html Page1 page1.html T1 T2", site.MainPage.Content);
            Assert.AreEqual("T2 page2.html T1 page2.html Page2 page2.html T1 T2", site.MainPage.SubPages[0].Content);
        }
    }
}