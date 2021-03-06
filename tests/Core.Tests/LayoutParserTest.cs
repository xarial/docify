﻿//*********************************************************************
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
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Base;
using Xarial.Docify.Core.Exceptions;

namespace Core.Tests
{
    public class LayoutParserTest
    {
        private LayoutParser m_Parser;

        [SetUp]
        public void Setup() 
        {
            var mock = new Mock<IDynamicContentTransformer>();
            mock.Setup(m => m.Transform(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ContextModel>()))
                .Returns(new Func<string, string, IContextModel, Task<string>>(
                    (c, k, m) => Task.FromResult(c)));

            m_Parser = new LayoutParser(mock.Object);
        }

        [Test]
        public void ContainsPlaceholder()
        {
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("Hello {{ content }}"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{ content }}"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{ content }} AAA"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("ABC {{ content }} aaa"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{    content }}"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("Hello {{ content }}"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{ content  }}"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{ content}} AAA"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("ABC {{content}} aaa"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{ content}} AAA\r\n{{ content }}"));
            Assert.DoesNotThrow(() => m_Parser.ValidateLayout("{{content }}"));
            Assert.Throws<LayoutMissingContentPlaceholder>(() => m_Parser.ValidateLayout("{content} Some Text"));
        }

        [Test]
        public async Task InsertContent()
        {
            var site = new Mock<ISite>().Object;
            var page = new Mock<IPage>().Object;

            Assert.AreEqual("Hello __replacement__", await m_Parser.InsertContent(new TemplateMock("", "Hello {{ content }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__", await m_Parser.InsertContent(new TemplateMock("", "{{ content }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__ AAA", await m_Parser.InsertContent(new TemplateMock("", "{{ content }} AAA"), "__replacement__", site, page, ""));
            Assert.AreEqual("ABC __replacement__ aaa", await m_Parser.InsertContent(new TemplateMock("", "ABC {{ content }} aaa"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__", await m_Parser.InsertContent(new TemplateMock("", "{{    content }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("Hello __replacement__", await m_Parser.InsertContent(new TemplateMock("", "Hello {{ content }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__", await m_Parser.InsertContent(new TemplateMock("", "{{ content  }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__ AAA", await m_Parser.InsertContent(new TemplateMock("", "{{ content}} AAA"), "__replacement__", site, page, ""));
            Assert.AreEqual("ABC __replacement__ aaa", await m_Parser.InsertContent(new TemplateMock("", "ABC {{content}} aaa"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__ AAA\r\n__replacement__", await m_Parser.InsertContent(new TemplateMock("", "{{ content}} AAA\r\n{{ content }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("__replacement__", await m_Parser.InsertContent(new TemplateMock("", "{{content }}"), "__replacement__", site, page, ""));
            Assert.AreEqual("{content} Some Text", await m_Parser.InsertContent(new TemplateMock("", "{content} Some Text"), "__replacement__", site, page, ""));
        }

        [Test]
        public async Task InsertContentNested()
        {
            var site = new Mock<ISite>().Object;
            var page = new Mock<IPage>().Object;

            var t1 = new TemplateMock("", "T1 {{ content }} T1");
            var t2 = new TemplateMock("", "T2 {{ content }} T2", null, t1);

            Assert.AreEqual("T1 T2 __replacement__ T2 T1", await m_Parser.InsertContent(t2, "__replacement__", site, page, ""));
        }
    }
}
