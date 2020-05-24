//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Autofac;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Threading.Tasks;
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;

namespace CLI.Tests
{
    public class CompilerTest
    {
        private ICompiler m_Compiler;

        [SetUp]
        public void Setup()
        {
            m_Compiler = new DocifyEngineMock(@"D:\src", @"D:\out", "www.xarial.com", "Test").Resove<ICompiler>();
        }
        
        [Test]
        public async Task NoTemplatePageTest()
        {
            var p1 = new PageMock("page1",
                "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n<div>@Model.Site.MainPage.SubPages.Count <a href=\"@Model.Page.Url\">Test</a></div>");
                        
            var p2 = new PageMock("page2",
                "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n<p>@Model.Site.MainPage.SubPages.Count</p>\n\n[page](@Model.Site.BaseUrl@Model.Page.Url)");
            
            p1.SubPages.Add(p2);

            var site = new Site("https://www.mysite.com", p1, null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<div>1 <a href=\"/\">Test</a></div>", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("<p>1</p>\n<p><a href=\"https://www.mysite.com/page2/\">page</a></p>", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
        }
        
        [Test]
        public async Task TemplatePageTest()
        {
            var site = new Site("",
                new PageMock("page",
                "My Page Content",
                new TemplateMock("t1", "TemplateText1{{ content }}TemplateText2")), null);
            
            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<p>TemplateText1<p>My Page Content</p>TemplateText2</p>", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }
        
        [Test]
        public async Task NestedTemplatePageTest()
        {
            var t2 = new TemplateMock("t2", "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n*T2* @Model.Page.Url {{ content }}_T2");
            var t1 = new TemplateMock("t1", "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n*T1* @Model.Page.Url {{ content }}_T1", null, t2);
            var t3 = new TemplateMock("t3", "T3{{ content }}T3");
            var t4 = new TemplateMock("t4", "T4{{ content }}T4", null, t3);

            var p1 = new PageMock("page1", "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n **Page1** @Model.Page.Url", t1);
            var p2 = new PageMock("page2", "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n **Page2** @Model.Page.Url", t1);
            var p3 = new PageMock("page3", "Page3", t4);
            var p4 = new PageMock("page4", "Page4", t4);

            p1.SubPages.Add(p2);
            p1.SubPages.Add(p3);
            p1.SubPages.Add(p4);

            var site = new Site("", p1, null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<p><em>T2</em> / <p><em>T1</em> / <p><strong>Page1</strong> /</p>_T1</p>_T2</p>", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("<p><em>T2</em> /page2/ <p><em>T1</em> /page2/ <p><strong>Page2</strong> /page2/</p>_T1</p>_T2</p>", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
            Assert.AreEqual("<p>T3<p>T4<p>Page3</p>T4</p>T3</p>", files.First(f => f.Location.ToId() == "page3::index.html").AsTextContent());
            Assert.AreEqual("<p>T3<p>T4<p>Page4</p>T4</p>T3</p>", files.First(f => f.Location.ToId() == "page4::index.html").AsTextContent());
        }

        [Test]
        public async Task IncludePageTest()
        {
            var p1 = new PageMock("page1",
                "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n*@Model.Site.MainPage.SubPages.Count* {% i1 %}");

            var p2 = new PageMock("page2",
                "@inherits TemplatePage<Xarial.Docify.Base.Context.IContextModel>\r\n@Model.Page.Url\r\n{% i1 p1: B %}\r\n{% i2 p2: X %}");

            p1.SubPages.Add(p2);

            var site = new Site("", p1, null);
            site.Includes.Add(new TemplateMock("i1", "@inherits TemplatePage<Xarial.Docify.Base.Context.IIncludeContextModel>\r\nSome Value\r\n@Model.Data[\"p1\"]", new Metadata() { { "p1", "A" } }));
            site.Includes.Add(new TemplateMock("i2", "@inherits TemplatePage<Xarial.Docify.Base.Context.IIncludeContextModel>\r\n**@Model.Page.Url**\r\n@Model.Data.Count", new Metadata() { { "p1", "A" }, { "p2", "X" } }));

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<p><em>1</em> Some Value\r\nA</p>", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
            Assert.AreEqual("<p>/page2/\nSome Value\r\nB\n**/page2/**\r\n2</p>", files.First(f => f.Location.ToId() == "page2::index.html").AsTextContent());
        }

        [Test]
        public async Task IncludeMultilineTest()
        {
            var p1 = new PageMock("page1",
                "abc {% i1 \r\n %}");
            
            var site = new Site("", p1, null);
            site.Includes.Add(new TemplateMock("i1", "Some Value"));

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<p>abc Some Value</p>", files.First(f => f.Location.ToId() == "index.html").Content);
        }

        [Test]
        public async Task IncludeLayoutTest()
        {
            var l1 = new TemplateMock("l1", "abc {% i1 { p1: B } %} klm {{ content }} xyz");

            var p1 = new PageMock("page1",
                "p1 {% i1 %}", l1);

            var site = new Site("", p1, null);
            site.Includes.Add(new TemplateMock("i1", "@inherits TemplatePage<Xarial.Docify.Base.Context.IIncludeContextModel>\r\nSome Value: @Model.Data[\"p1\"]", new Metadata() { { "p1", "A" } }));
            
            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<p>abc Some Value: B klm <p>p1 Some Value: A</p> xyz</p>", files.First(f => f.Location.ToId() == "index.html").AsTextContent());
        }

        [Test]
        public async Task BinaryAssetTest()
        {
            var site = new Site("", new PageMock("page1", ""), null);
            var asset = new AssetMock("file.bin", new byte[] { 1, 2, 3 });
            site.MainPage.Assets.Add(asset);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.That(new byte[] { 1, 2, 3 }.SequenceEqual(files.First(a => a.Location.ToId() == "file.bin").Content));
        }

        [Test]
        public async Task TextAssetTest()
        {
            var site = new Site("", new PageMock("page1", ""), null);
            var asset = new AssetMock("file.txt", ContentExtension.ToByteArray("test"));
            site.MainPage.Assets.Add(asset);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("test", files.First(f => f.Location.ToId() == "file.txt").Content);
        }

        [Test]
        public async Task SubPageAssetTest()
        {
            var site = new Site("", new PageMock("", ""), null);
            var asset = new AssetMock("file.txt", ContentExtension.ToByteArray("test"));
            var p2 = new PageMock("p2", "");
            site.MainPage.SubPages.Add(p2);
            p2.Assets.Add(asset);

            var files = await m_Compiler.Compile(site).ToListAsync();

            var a = files.FirstOrDefault(f => f.Location.ToId() == "p2::file.txt");

            Assert.IsNotNull(a);
            Assert.AreEqual("test", a.AsTextContent());
        }

        [Test]
        public async Task OpenIncludeTest()
        {
            var site = new Site("", new PageMock("page1", "abc {% x *test*"), null);

            var files = await m_Compiler.Compile(site).ToListAsync();

            Assert.AreEqual("<p>abc {% x <em>test</em></p>", files.First(f => f.Location.ToId() == "index.html").Content);
        }
    }
}