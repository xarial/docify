//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Tests.Common.Mocks;
using Core.Tests.Properties;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;

namespace Core.Tests
{
    public class BaseCompilerTest
    {
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup() 
        {
            string GetPageName(IContextPage page) 
            {
                var name = page.Url.Split("/", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (string.IsNullOrEmpty(name))
                {
                    name = "index";
                }
                name += ".html";

                return name;
            }

            var contTransMock = new Mock<IContentTransformer>();
            contTransMock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IContextModel>()))
                .Returns<string, string, IContextModel>((c, k, m) => Task.FromResult(
                    c.Replace("_FN_", GetPageName((m as ContextModel).Page))
                    .Replace("_CC_", (m as ContextModel).Site.MainPage.SubPages.Count().ToString())));

            var layoutMock = new Mock<ILayoutParser>();

            layoutMock.Setup(m => m.ContainsPlaceholder(It.IsAny<string>()))
                .Returns<string>(c => c.Contains("_C_"));

            layoutMock.Setup(m => m.InsertContent(It.IsAny<Template>(), It.IsAny<string>(), It.IsAny<IContextModel>()))
                .Returns<Template, string, IContextModel>((t, c, m) =>
                {
                    string r = c;
                    ITemplate tt = t;

                    while (tt != null)
                    {
                        r = tt.RawContent.Replace("_C_", r)
                        .Replace("_FN_", GetPageName((m as ContextModel).Page))
                        .Replace("_CC_", (m as ContextModel).Site.MainPage.SubPages.Count().ToString());

                        tt = tt.Layout;
                    }

                    return Task.FromResult(r);
                });

            var includesHandlerMock = new Mock<IIncludesHandler>();
            includesHandlerMock.Setup(m => m.ReplaceAll(It.IsAny<string>(), It.IsAny<Site>(), 
                It.IsAny<Page>(), It.IsAny<string>()))
                .Returns<string, Site, Page, string>((c, s, p, u) => Task.FromResult(c));

            m_Compiler = new BaseCompiler(new BaseCompilerConfig(),
                new Mock<ILogger>().Object,
                layoutMock.Object,
                includesHandlerMock.Object,
                contTransMock.Object);
        }

        [Test]
        public async Task Compile_SinglePageTest()
        {
            var site = new Site("",
                new PageMock("", "abc _CC_ _FN_ test"), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("abc 0 index.html test", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_MultipleNestedPagesTest()
        {
            var p1 = new PageMock("", "P1");

            var site = new Site("", p1, null);

            var p2 = new PageMock("page2", "P2");
            p1.SubPages.Add(p2);
            p2.SubPages.Add(new PageMock("page3", "P3"));
            var p4 = new PageMock("page4", "P4");
            p2.SubPages.Add(p4);
            p4.SubPages.Add(new PageMock("page5", "P5"));

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("P1", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("P2", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
            Assert.AreEqual("P3", files.First(f => f.Location.ToId() == "page2::page3::index.html").AsTextContent());
            Assert.AreEqual("P4", files.First(f => f.Location.ToId() == "page2::page4::index.html").AsTextContent());
            Assert.AreEqual("P5", files.First(f => f.Location.ToId() == "page2::page4::page5::index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_SimpleTemplatePageTest()
        {
            var site = new Site("",
                new PageMock("", "My Page Content",
                new TemplateMock("t1", "TemplateText1 _C_ TemplateText2")), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("TemplateText1 My Page Content TemplateText2", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_NestedSimpleTemplatePageTest()
        {
            var site = new Site("",
                new PageMock("",
                "My Page Content",
                new TemplateMock("t1", "T1 _C_ T1", null,
                new TemplateMock("t2", "T2 _C_ T2"))), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("T2 T1 My Page Content T1 T2", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_NestedTemplateMultiPageTest()
        {
            var t2 = new TemplateMock("t2", "T2 _FN_ _C_ T2");
            var t1 = new TemplateMock("t1", "T1 _FN_ _C_ T1", null, t2);

            var p1 = new PageMock("page1", "Page1 _FN_", t1);

            p1.SubPages.Add(new PageMock("page2", "Page2 _FN_", t1));

            var site = new Site("", p1, null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("T2 index.html T1 index.html Page1 index.html T1 T2", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("T2 page2.html T1 page2.html Page2 page2.html T1 T2", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
        }
    }
}