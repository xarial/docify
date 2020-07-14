//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Tests.Common.Mocks;
using Core.Tests.Properties;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Core.Tests
{
    public class BaseCompilerTest
    {
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup() 
        {
            string GetPageName(string url) 
            {
                var name = url.Split("/", StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
                if (string.IsNullOrEmpty(name))
                {
                    name = "index";
                }
                name += ".html";

                return name;
            }

            var contTransMock = new Mock<IStaticContentTransformer>();
            contTransMock.Setup(m => m.Transform(It.IsAny<string>()))
                .Returns<string>(c => Task.FromResult($"CT_{c}_CT"));

            var layoutMock = new Mock<ILayoutParser>();

            layoutMock.Setup(m => m.ValidateLayout(It.IsAny<string>()))
                .Callback<string>(c =>
                {
                    if (!c.Contains("_C_")) 
                    {
                        throw new Exception();
                    }
                });

            layoutMock.Setup(m => m.InsertContent(
                It.IsAny<ITemplate>(), It.IsAny<string>(), It.IsAny<ISite>(), It.IsAny<IPage>(), It.IsAny<string>()))
                .Returns<ITemplate, string, ISite, IPage, string>((t, c, s, p, u) =>
                {
                    string r = c;
                    ITemplate tt = t;

                    while (tt != null)
                    {
                        r = tt.RawContent.Replace("_C_", r)
                        .Replace("_FN_", GetPageName(u))
                        .Replace("_CC_", s.MainPage.SubPages.Count().ToString());

                        tt = tt.Layout;
                    }

                    return Task.FromResult(r);
                });

            var includesHandlerMock = new Mock<IIncludesHandler>();
            includesHandlerMock.Setup(m => m.ResolveAll(It.IsAny<string>(), It.IsAny<Site>(), 
                It.IsAny<Page>(), It.IsAny<string>()))
                .Returns<string, Site, Page, string>((c, s, p, u) => Task.FromResult(c));
            
            m_Compiler = new BaseCompiler(new BaseCompilerConfig(new Configuration()),
                new Mock<ILogger>().Object,
                layoutMock.Object,
                includesHandlerMock.Object,
                contTransMock.Object,
                new Mock<ICompilerExtension>().Object);
        }

        [Test]
        public async Task Compile_SinglePageTest()
        {
            var site = new Site("", "",
                new PageMock("", "abc _CC_ _FN_ test"), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("CT_abc _CC_ _FN_ test_CT", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_MultipleNestedPagesTest()
        {
            var p1 = new PageMock("", "P1");

            var site = new Site("", "", p1, null);

            var p2 = new PageMock("page2", "P2");
            p1.SubPages.Add(p2);
            p2.SubPages.Add(new PageMock("page3", "P3"));
            var p4 = new PageMock("page4", "P4");
            p2.SubPages.Add(p4);
            p4.SubPages.Add(new PageMock("page5", "P5"));

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("CT_P1_CT", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("CT_P2_CT", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
            Assert.AreEqual("CT_P3_CT", files.First(f => f.Location.ToId() == "page2::page3::index.html").AsTextContent());
            Assert.AreEqual("CT_P4_CT", files.First(f => f.Location.ToId() == "page2::page4::index.html").AsTextContent());
            Assert.AreEqual("CT_P5_CT", files.First(f => f.Location.ToId() == "page2::page4::page5::index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_NonDefaultPageTest()
        {
            var site = new Site("", "", new PageMock("", "P1"), null);
            var p2 = new PageMock("page2.html", "P2");
            site.MainPage.SubPages.Add(p2);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("CT_P1_CT", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("CT_P2_CT", files.First(f => f.Location.ToId() == "page2.html").AsTextContent());
        }

        [Test]
        public async Task Compile_SimpleTemplatePageTest()
        {
            var site = new Site("", "",
                new PageMock("", "My Page Content",
                new TemplateMock("t1", "TemplateText1 _C_ TemplateText2")), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("TemplateText1 CT_My Page Content_CT TemplateText2", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_NestedSimpleTemplatePageTest()
        {
            var site = new Site("", "",
                new PageMock("",
                "My Page Content",
                new TemplateMock("t1", "T1 _C_ T1", null,
                new TemplateMock("t2", "T2 _C_ T2"))), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("T2 T1 CT_My Page Content_CT T1 T2", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_NestedTemplateMultiPageTest()
        {
            var t2 = new TemplateMock("t2", "T2 _FN_ _C_ T2");
            var t1 = new TemplateMock("t1", "T1 _FN_ _C_ T1", null, t2);

            var p1 = new PageMock("page1", "Page1 _FN_", t1);

            p1.SubPages.Add(new PageMock("page2", "Page2 _FN_", t1));

            var site = new Site("", "", p1, null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("T2 index.html T1 index.html CT_Page1 index.html_CT T1 T2", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("T2 page2.html T1 page2.html CT_Page2 page2.html_CT T1 T2", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
        }

        [Test]
        public async Task Compile_ExcludePhantomPage() 
        {
            var p1 = new PageMock("page1", "p1");
            var p2 = new PhantomPageMock("page2");
            var p3 = new PageMock("page3", "p3");
            var p4 = new PhantomPageMock("page4");
            var p5 = new PageMock("page5", "p5");
            var p6 = new PageMock("page6", "p6");
            
            p1.SubPages.Add(p2);
            p1.SubPages.Add(p5);
            p2.SubPages.Add(p3);
            p2.SubPages.Add(p4);
            p4.SubPages.Add(p6);

            var site = new Site("", "", p1, null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual(4, files.Count);
            Assert.IsNotNull(files.FirstOrDefault(f => f.Location.ToId() == "index.html"));
            Assert.IsNotNull(files.FirstOrDefault(f => f.Location.ToId() == "page5::index.html"));
            Assert.IsNotNull(files.FirstOrDefault(f => f.Location.ToId() == "page2::page3::index.html"));
            Assert.IsNotNull(files.FirstOrDefault(f => f.Location.ToId() == "page2::page4::page6::index.html"));
        }
    }
}