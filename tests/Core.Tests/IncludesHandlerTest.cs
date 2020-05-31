//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Tests.Common.Mocks;
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
using Xarial.Docify.Core.Plugin.Extensions;

namespace Core.Tests
{
    public class IncludesHandlerTest
    {
        private IncludesHandler m_Handler;

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

            var mock = new Mock<IDynamicContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) => Task.FromResult(
                        $"{c}_{GetPageName((m as ContextModel).Page)}_{string.Join(",", m.Data.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}").ToArray())}")));

            var incExt = new Mock<IIncludesHandlerExtension>();
            incExt.Setup(m => m.ResolveInclude(It.IsAny<string>(), It.IsAny<IMetadata>(), It.IsAny<IPage>()))
                .Returns((string c, IMetadata m, IPage p) => throw new MissingIncludeException(c));

            m_Handler = new IncludesHandler(mock.Object, incExt.Object);
        }

        [Test]
        public async Task ParseParameters_SingleLine()
        {
            IContextMetadata data = null;

            var mock = new Mock<IDynamicContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) =>
                    {
                        data = m.Data;
                        return Task.FromResult("");
                    }));
                       
            var handler = new IncludesHandler(mock.Object, new Mock<IIncludesHandlerExtension>().Object);

            var s = new Site("", new PageMock("", ""), null);
            s.Includes.Add(new TemplateMock("include", ""));
            
            IContextMetadata p1, p2, p3;

            await handler.ResolveAll("{% include a1: A %}", s, s.MainPage, "");
            p1 = data;

            await handler.ResolveAll("{%  include  a1: A %}", s, s.MainPage, "");
            p2 = data;

            await handler.ResolveAll("{% include { a1: A, a2: 0.2 } %}", s, s.MainPage, "");
            p3 = data;

            Assert.AreEqual(1, p1.Count);
            Assert.AreEqual("A", p1["a1"]);

            Assert.AreEqual(1, p2.Count);
            Assert.AreEqual("A", p2["a1"]);

            Assert.AreEqual(2, p3.Count);
            Assert.AreEqual("A", p3["a1"]);
            Assert.AreEqual("0.2", p3["a2"]);
        }

        [Test]
        public async Task ParseParameters_MultipleLine()
        {
            IContextMetadata data = null;

            var mock = new Mock<IDynamicContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) =>
                    {
                        data = m.Data;
                        return Task.FromResult("");
                    }));

            var handler = new IncludesHandler(mock.Object, new Mock<IIncludesHandlerExtension>().Object);

            var s = new Site("", new PageMock("", ""), null);
            s.Includes.Add(new TemplateMock("include", ""));
            
            await handler.ResolveAll("{% include a1: A\r\na2: B\r\na3:\r\n    - X\r\n    - Y %}", s, s.MainPage, "");
            
            Assert.AreEqual(3, data.Count);
            Assert.AreEqual("A", data["a1"]);
            Assert.AreEqual("B", data["a2"]);
            Assert.AreEqual(2, (data["a3"] as List<object>).Count);
            Assert.AreEqual("X", (data["a3"] as List<object>)[0]);
            Assert.AreEqual("Y", (data["a3"] as List<object>)[1]);
        }

        [Test]
        public async Task ParseParameters_NoParameters()
        {
            IContextMetadata data = null;

            var mock = new Mock<IDynamicContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) =>
                    {
                        data = m.Data;
                        return Task.FromResult("");
                    }));

            var handler = new IncludesHandler(mock.Object, new Mock<IIncludesHandlerExtension>().Object);

            var s = new Site("", new PageMock("", ""), null);
            s.Includes.Add(new TemplateMock("include", ""));

            await handler.ResolveAll("{% include %}", s, s.MainPage, "");

            Assert.AreEqual(0, data.Count);
        }

        [Test]
        public async Task Render_SimpleParameters()
        {
            var p1 = new PageMock("", "");
            var p2 = new PageMock("page2", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));
            p1.SubPages.Add(p2);
            
            var res1 = await m_Handler.ResolveAll("{% i1 a1: A %}", s, p1, "/page1/");
            var res2 = await m_Handler.ResolveAll("{% i1 a2: B %}", s, p2, "/page1/page2/");

            Assert.AreEqual("abc_page1.html_a1=A", res1);
            Assert.AreEqual("abc_page2.html_a2=B", res2);
        }

        [Test]
        public async Task Render_MergedIncludeParameters()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc", new Metadata() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.ResolveAll("{% i1 { a1: X, a3: Y} %}", s, p1, "/page1/");

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

            var p1 = new PageMock("", "", md);
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc", new Metadata() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.ResolveAll("{% i1 { a1: X, a3: Y } %}", s, p1, "/page1/");

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

            var p1 = new PageMock("", "");
            var s = new Site("", p1, conf);
            s.Includes.Add(new TemplateMock("i1", "abc", new Metadata() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.ResolveAll("{% i1 { a1: X, a3: Y } %}", s, p1, "/page1/");

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

            var p1 = new PageMock("", "", md);
            var s = new Site("", p1, conf);
            s.Includes.Add(new TemplateMock("i1", "abc", new Metadata() { { "a1", "T1" }, { "a2", "T2" }, { "a3", "" }, { "a4", "T4" } }));

            var res1 = await m_Handler.ResolveAll("{% i1 { a1: I1, a2:, a4: } %}", s, p1, "/page1/");

            Assert.AreEqual("abc_page1.html_a1=I1,a2=P2,a3=S3,a4=T4", res1);
        }

        [Test]
        public async Task Render_MissingIncludes()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));

            MissingIncludeException e = null;

            try
            {
                await m_Handler.ResolveAll("{% i2 %}", s, p1, "/page1/");
            }
            catch (IncludeResolveException ex)
            {
                e = ex.InnerException as MissingIncludeException;
            }

            Assert.IsNotNull(e);
        }

        [Test]
        public async Task Render_PluginIncludes()
        {
            var extMock = new Mock<IIncludesHandlerExtension>();
            extMock.Setup(m => m.ResolveInclude(It.IsAny<string>(), It.IsAny<IMetadata>(), It.IsAny<IPage>()))
                .Returns((string i, IMetadata m, IPage p) => Task.FromResult($"_{i}_render-result"));

            var includesHandler = new IncludesHandler(new Mock<IDynamicContentTransformer>().Object,
                extMock.Object);

            var p1 = new PageMock("", "{% plugin-include { param1: x, param2: b} %}");
            var s = new Site("", p1, null);

            var res = await includesHandler.ResolveAll("{% plugin-include %}", s, p1, "/page1/");

            Assert.AreEqual("_plugin-include_render-result", res);
        }

        [Test]
        public async Task ReplaceAll_NewLineSingleLineInclude()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));

            var res = await m_Handler.ResolveAll("abc\r\n{% i1 a1: x %}\r\nxyz", s, p1, "/page1/");

            Assert.AreEqual("abc\r\nabc_page1.html_a1=x\r\nxyz", res);
        }

        [Test]
        public async Task ReplaceAll_InlineInclude()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));

            var res = await m_Handler.ResolveAll("abc{% i1 a1: x %}xyz", s, p1, "/page1/");

            Assert.AreEqual("abcabc_page1.html_a1=xxyz", res);
        }

        [Test]
        public async Task ReplaceAll_MultilineInlineInclude()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));

            var res = await m_Handler.ResolveAll("abc{% i1 a1: x\r\na2: y %}xyz", s, p1, "/page1/");

            Assert.AreEqual("abcabc_page1.html_a1=x,a2=yxyz", res);
        }

        [Test]
        public async Task ReplaceAll_HtmlTagsInclude()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));

            var res1 = await m_Handler.ResolveAll("<div>{% i1 a1: x %}</div>", s, p1, "/page1/");
            var res2 = await m_Handler.ResolveAll("<div>\r\n{% i1 a1: x %}\r\n</div>", s, p1, "/page1/");

            Assert.AreEqual("<div>abc_page1.html_a1=x</div>", res1);
            Assert.AreEqual("<div>\r\nabc_page1.html_a1=x\r\n</div>", res2);
        }

        [Test]
        public async Task ReplaceAll_MultipleIncludes()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));
            s.Includes.Add(new TemplateMock("i2", "xyz"));

            var res1 = await m_Handler.ResolveAll("__{% i1 a1: x %}__{% i2 a2: y %}++{% i1 a1: z %}--", s, p1, "/page1/");

            Assert.AreEqual("__abc_page1.html_a1=x__xyz_page1.html_a2=y++abc_page1.html_a1=z--", res1);
        }

        [Test]
        public async Task ReplaceAll_NestedIncludes()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));
            s.Includes.Add(new TemplateMock("i2", "xyz{% i1 a1: z %}"));

            var res1 = await m_Handler.ResolveAll("__{% i1 a1: x %}__{% i2 a2: y %}", s, p1, "/page1/");

            Assert.AreEqual("__abc_page1.html_a1=x__xyzabc_page1.html_a1=z_page1.html_a2=y", res1);
        }

        [Test]
        public async Task ReplaceAll_NestedMultiLevelIncludes()
        {
            var p1 = new PageMock("", "");
            var s = new Site("", p1, null);
            s.Includes.Add(new TemplateMock("i1", "abc"));
            s.Includes.Add(new TemplateMock("i2", "xyz{% i1 a1: z %}"));
            s.Includes.Add(new TemplateMock("i3", "abc{% i2 %}"));

            var res1 = await m_Handler.ResolveAll("{% i3 %}__{% i1 %}", s, p1, "/page1/");

            Assert.AreEqual("abcxyzabc_page1.html_a1=z_page1.html__page1.html___abc_page1.html_", res1);
        }
    }
}
