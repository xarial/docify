//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class IncludesHandlerTest
    {
        private IncludesHandler m_Handler;

        [SetUp]
        public void Setup()
        {
            m_Handler = CreateNewIncludesHandler();
        }

        [Test]
        public void ParseParameters_SingleLine() 
        {
            string n1, n2, n3;
            IMetadata p1, p2, p3;
            
            m_Handler.ParseParameters("include a1: A", out n1, out p1);
            m_Handler.ParseParameters(" include  a1: A", out n2, out p2);
            m_Handler.ParseParameters("include { a1: A, a2: 0.2 }", out n3, out p3);

            Assert.AreEqual("include", n1);
            Assert.AreEqual(1, p1.Count);
            Assert.AreEqual("A", p1["a1"]);

            Assert.AreEqual("include", n2);
            Assert.AreEqual(1, p2.Count);
            Assert.AreEqual("A", p2["a1"]);

            Assert.AreEqual("include", n3);
            Assert.AreEqual(2, p3.Count);
            Assert.AreEqual("A", p3["a1"]);
            Assert.AreEqual("0.2", p3["a2"]);
        }

        [Test]
        public void ParseParameters_MultipleLine()
        {
            string n1;
            IMetadata p1;

            m_Handler.ParseParameters("include a1: A\r\na2: B\r\na3:\r\n    - X\r\n    - Y", out n1, out p1);

            Assert.AreEqual("include", n1);
            Assert.AreEqual(3, p1.Count);
            Assert.AreEqual("A", p1["a1"]);
            Assert.AreEqual("B", p1["a2"]);
            Assert.AreEqual(2, (p1["a3"] as List<object>).Count);
            Assert.AreEqual("X", (p1["a3"] as List<object>)[0]);
            Assert.AreEqual("Y", (p1["a3"] as List<object>)[1]);
        }

        [Test]
        public void ParseParameters_NoParameters()
        {
            string n1;
            IMetadata p1;

            m_Handler.ParseParameters("include", out n1, out p1);

            Assert.AreEqual("include", n1);
            Assert.AreEqual(0, p1.Count);
        }

        [Test]
        public async Task Render_SimpleParameters()
        {
            var p1 = new Page("", "");
            var p2 = new Page("page2", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));
            p1.SubPages.Add(p2);

            var res1 = await m_Handler.Render("i1", new Metadata() { { "a1", "A" } }, s, p1, "/page1/");
            var res2 = await m_Handler.Render("i1", new Metadata() { { "a2", "B" } }, s, p2, "/page1/page2/");

            Assert.AreEqual("abc_page1.html_a1=A", res1);
            Assert.AreEqual("abc_page2.html_a2=B", res2);
        }

        [Test]
        public async Task Render_MergedIncludeParameters()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc", new Metadata() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.Render("i1", new Metadata() { { "a1", "X" }, { "a3", "Y" } }, s, p1, "/page1/");

            Assert.AreEqual("abc_page1.html_a1=X,a2=B,a3=Y", res1);
        }

        [Test]
        public async Task Render_MergedPageParameters()
        {
            var md = new Metadata();
            md.Add("$i1", new Dictionary<string, object>() 
            {
                { "a1", "Z" },
                { "a4", "J" }
            });

            var p1 = new Page("", "", md);
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc", new Metadata() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.Render("i1", new Metadata() { { "a1", "X" }, { "a3", "Y" } }, s, p1, "/page1/");

            Assert.AreEqual("abc_page1.html_a1=X,a2=B,a3=Y,a4=J", res1);
        }

        [Test]
        public async Task Render_MergedSiteParameters()
        {
            var conf = new Configuration();
            conf.Add("$i1", new Dictionary<string, object>()
            {
                { "a1", "Z" },
                { "a4", "J" }
            });

            var p1 = new Page("", "");
            var s = new Site("", p1, conf);
            s.Includes.Add(new Template("i1", "abc", new Metadata() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.Render("i1", new Metadata() { { "a1", "X" }, { "a3", "Y" } }, s, p1, "/page1/");

            Assert.AreEqual("abc_page1.html_a1=X,a2=B,a3=Y,a4=J", res1);
        }

        [Test]
        public async Task Render_MergedParametersHierarchy()
        {
            var conf = new Configuration();
            conf.Add("$i1", new Dictionary<string, object>()
            {
                { "a1", "S1" },
                { "a2", "S2" },
                { "a3", "S3" }
            });

            var md = new Metadata();
            md.Add("$i1", new Dictionary<string, object>()
            {
                { "a1", "P1" },
                { "a2", "P2" },
                { "a3", "" }
            });

            var p1 = new Page("", "", md);
            var s = new Site("", p1, conf);
            s.Includes.Add(new Template("i1", "abc", new Metadata() { { "a1", "T1" }, { "a2", "T2" }, { "a3", "" }, { "a4", "T4" } }));

            var res1 = await m_Handler.Render("i1", new Metadata() { { "a1", "I1" }, { "a2", null }, { "a4", "" } }, s, p1, "/page1/");

            Assert.AreEqual("abc_page1.html_a1=I1,a2=P2,a3=S3,a4=T4", res1);
        }

        [Test]
        public void Render_MissingIncludes()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));

            Assert.ThrowsAsync<MissingIncludeException>(() => m_Handler.Render("i2", new Metadata(), s, p1, "/page1/"));
        }

        [Test]
        public async Task Render_PluginIncludes()
        {
            var includesHandler = CreateNewIncludesHandler();

            var includePluginMock = new Mock<IIncludeResolverPlugin>();
            includePluginMock.SetupGet(x => x.IncludeName).Returns("plugin-include");
            includePluginMock.Setup(x => x.ResolveInclude(It.IsAny<IMetadata>(), It.IsAny<IPage>()))
                .Returns(new Func<IMetadata, IPage, Task<string>>((m, p) => Task.FromResult("render-result")));

            includesHandler.GetType().GetField("m_IncludeResolverPlugins", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(includesHandler, new IIncludeResolverPlugin[] { includePluginMock.Object });

            var p1 = new Page("", "{% plugin-include { param1: x, param2: b} %}");
            var s = new Site("", p1, null);

            var res = await includesHandler.Render("plugin-include", new Metadata(), s, p1, "/page1/");

            Assert.AreEqual("render-result", res);
        }

        [Test]
        public async Task ReplaceAll_NewLineSingleLineInclude()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));

            var res = await m_Handler.ReplaceAll("abc\r\n{% i1 a1: x %}\r\nxyz", s, p1, "/page1/");

            Assert.AreEqual("abc\r\nabc_page1.html_a1=x\r\nxyz", res);
        }

        [Test]
        public async Task ReplaceAll_InlineInclude()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));

            var res = await m_Handler.ReplaceAll("abc{% i1 a1: x %}xyz", s, p1, "/page1/");

            Assert.AreEqual("abcabc_page1.html_a1=xxyz", res);
        }

        [Test]
        public async Task ReplaceAll_MultilineInlineInclude()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));

            var res = await m_Handler.ReplaceAll("abc{% i1 a1: x\r\na2: y %}xyz", s, p1, "/page1/");

            Assert.AreEqual("abcabc_page1.html_a1=x,a2=yxyz", res);
        }

        [Test]
        public async Task ReplaceAll_HtmlTagsInclude()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));

            var res1 = await m_Handler.ReplaceAll("<div>{% i1 a1: x %}</div>", s, p1, "/page1/");
            var res2 = await m_Handler.ReplaceAll("<div>\r\n{% i1 a1: x %}\r\n</div>", s, p1, "/page1/");

            Assert.AreEqual("<div>abc_page1.html_a1=x</div>", res1);
            Assert.AreEqual("<div>\r\nabc_page1.html_a1=x\r\n</div>", res2);
        }

        [Test]
        public async Task ReplaceAll_MultipleIncludes()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));
            s.Includes.Add(new Template("i2", "xyz"));

            var res1 = await m_Handler.ReplaceAll("__{% i1 a1: x %}__{% i2 a2: y %}++{% i1 a1: z %}--", s, p1, "/page1/");

            Assert.AreEqual("__abc_page1.html_a1=x__xyz_page1.html_a2=y++abc_page1.html_a1=z--", res1);
        }

        [Test]
        public async Task ReplaceAll_NestedIncludes()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));
            s.Includes.Add(new Template("i2", "xyz{% i1 a1: z %}"));

            var res1 = await m_Handler.ReplaceAll("__{% i1 a1: x %}__{% i2 a2: y %}", s, p1, "/page1/");

            Assert.AreEqual("__abc_page1.html_a1=x__xyzabc_page1.html_a1=z_page1.html_a2=y", res1);
        }

        [Test]
        public async Task ReplaceAll_NestedMultiLevelIncludes()
        {
            var p1 = new Page("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new Template("i1", "abc"));
            s.Includes.Add(new Template("i2", "xyz{% i1 a1: z %}"));
            s.Includes.Add(new Template("i3", "abc{% i2 %}"));

            var res1 = await m_Handler.ReplaceAll("{% i3 %}__{% i1 %}", s, p1, "/page1/");

            Assert.AreEqual("abcxyzabc_page1.html_a1=z_page1.html__page1.html___abc_page1.html_", res1);
        }

        private IncludesHandler CreateNewIncludesHandler()
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

            var mock = new Mock<IContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) => Task.FromResult(
                        $"{c}_{GetPageName((m as ContextModel).Page)}_{string.Join(",", (m as IncludeContextModel).Data.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}").ToArray())}")));

            return new IncludesHandler(mock.Object);
        }
    }
}
