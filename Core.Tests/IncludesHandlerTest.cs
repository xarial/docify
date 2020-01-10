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
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class IncludesHandlerTest
    {
        private IncludesHandler m_Handler;

        [SetUp]
        public void Setup() 
        {
            var mock = new Mock<IContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) => Task.FromResult(
                        $"{c}_{(m as ContextModel).Page.Key}_{string.Join(",", (m as IncludesContextModel).Parameters.OrderBy(p => p.Key).Select(p => $"{p.Key}={p.Value}").ToArray())}")));

            m_Handler = new IncludesHandler(mock.Object);
        }

        [Test]
        public void ParseParameters_SingleLine() 
        {
            string n1, n2, n3;
            Dictionary<string, dynamic> p1, p2, p3;
            
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
            Dictionary<string, dynamic> p1;

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
            Dictionary<string, dynamic> p1;

            m_Handler.ParseParameters("include", out n1, out p1);

            Assert.AreEqual("include", n1);
            Assert.AreEqual(0, p1.Count);
        }

        [Test]
        public async Task Insert_SimpleParameters()
        {
            var p1 = new Page(Location.FromPath("page1.html"), "");
            var p2 = new Page(Location.FromPath("page2.html"), "");
            var s = new Site("", p1);
            s.Includes.Add(new Template("i1", "abc"));
            p1.SubPages.Add(p2);

            var res1 = await m_Handler.Insert("i1", new Dictionary<string, dynamic>() { { "a1", "A" } }, s, p1);
            var res2 = await m_Handler.Insert("i1", new Dictionary<string, dynamic>() { { "a2", "B" } }, s, p2);

            Assert.AreEqual("abc_page1.html_a1=A", res1);
            Assert.AreEqual("abc_page2.html_a2=B", res2);
        }

        [Test]
        public async Task Insert_MergedParameters()
        {
            var p1 = new Page(Location.FromPath("page1.html"), "");
            var s = new Site("", p1);
            s.Includes.Add(new Template("i1", "abc", new Dictionary<string, dynamic>() { { "a1", "A" }, { "a2", "B" } }));

            var res1 = await m_Handler.Insert("i1", new Dictionary<string, dynamic>() { { "a1", "X" }, { "a3", "Y" } }, s, p1);

            Assert.AreEqual("abc_page1.html_a1=X,a2=B,a3=Y", res1);
        }

        [Test]
        public void Insert_MissingIncludes()
        {
            var p1 = new Page(Location.FromPath("page1.html"), "");
            var s = new Site("", p1);
            s.Includes.Add(new Template("i1", "abc"));

            Assert.ThrowsAsync<MissingIncludeException>(() => m_Handler.Insert("i2", new Dictionary<string, dynamic>(), s, p1));
        }
    }
}
