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
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Compiler.MarkdigMarkdownParser;
using Xarial.Docify.Core.Compiler;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Compiler.Context;

namespace Core.Tests
{
    public class MarkdigMarkdownIncludesExtensionTest
    {
        private delegate Task ParseParametersDelegate(string rawContent, out string name, out Metadata param);
        private delegate Task<string> InsertDelegate(string name, Metadata param, Site site, Page page);

        private MarkdigMarkdownContentTransformer m_Parser;

        [SetUp]
        public void Setup() 
        {
            var paramsParserMock = new Moq.Mock<IIncludesHandler>();
            paramsParserMock.Setup(m => m.ParseParameters(It.IsAny<string>(), out It.Ref<string>.IsAny,
                out It.Ref<Metadata>.IsAny)).Returns(
                new ParseParametersDelegate((string rawContent, out string name, out Metadata param) =>
                {
                    name = rawContent.Replace("\n", " ").Trim();
                    param = new Metadata() { { "A", "B" } };
                    return Task.CompletedTask;
                }));

            paramsParserMock.Setup(m => m.Insert(It.IsAny<string>(),
                It.IsAny<Metadata>(), It.IsAny<Site>(), It.IsAny<Page>())).Returns(
                new InsertDelegate((n, p, s, pg) =>
                {
                    return Task.FromResult($"[{n}: {p.ElementAt(0).Key}={p.ElementAt(0).Value}]");
                }));

            m_Parser = new MarkdigMarkdownContentTransformer(paramsParserMock.Object);
        }

        [Test]
        public async Task Transform_NewLineSingleLineInclude() 
        {            
            var res = await m_Parser.Transform("abc\r\n{% include some value %}\r\nxyz", "", new ContextModel(null, null));

            Assert.AreEqual("<p>abc\n[include some value: A=B]\nxyz</p>", res);
        }

        [Test]
        public async Task Transform_InlineInclude()
        {
            var res = await m_Parser.Transform("abc{% include some value %}xyz", "", new ContextModel(null, null));

            Assert.AreEqual("<p>abc[include some value: A=B]xyz</p>", res);
        }

        [Test]
        public async Task Transform_MultilineInlineInclude()
        {
            var res = await m_Parser.Transform("abc{% include some\r\nvalue %}xyz", "", new ContextModel(null, null));

            Assert.AreEqual("<p>abc[include some value: A=B]xyz</p>", res);
        }

        [Test]
        public async Task Transform_HtmlTagsInclude()
        {
            var res1 = await m_Parser.Transform("<div>{% include some\r\nvalue %}</div>", "", new ContextModel(null, null));
            var res2 = await m_Parser.Transform("<div>\r\n{% include some\r\nvalue %}\r\n</div>", "", new ContextModel(null, null));

            Assert.AreEqual("<div>[include some value: A=B]</div>", res1);
            Assert.AreEqual("<div>\r\n[include some value: A=B]\r\n</div>", res2);
        }

        [Test]
        public void Transform_NotClosedInclude()
        {
            Assert.ThrowsAsync<NotClosedIncludeException>(() => m_Parser.Transform("abc{% include some\r\nvalue xyz", "", new ContextModel(null, null)));
        }
    }
}
