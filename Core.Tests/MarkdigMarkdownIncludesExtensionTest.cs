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
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;

namespace Core.Tests
{
    public class MarkdigMarkdownIncludesExtensionTest
    {
        private delegate Task ParseParametersDelegate(string rawContent, out string name, out Dictionary<string, dynamic> param);
        private delegate Task<string> InsertDelegate(string name, Dictionary<string, dynamic> param, IEnumerable<Template> includes);

        [Test]
        public async Task Transform_NewLineSingleLineInclude() 
        {
            var paramsParserMock = new Moq.Mock<IIncludesHandler>();
            paramsParserMock.Setup(m => m.ParseParameters(It.IsAny<string>(), out It.Ref<string>.IsAny,
                out It.Ref<Dictionary<string, dynamic>>.IsAny)).Returns(
                new ParseParametersDelegate((string rawContent, out string name, out Dictionary<string, dynamic> param) =>
                {
                    var data = rawContent.Split(" ");

                    name = data[0];
                    param = data.Skip(1).ToDictionary(d => d, d => (dynamic)"");
                    return Task.CompletedTask;
                }));

            paramsParserMock.Setup(m => m.Insert(It.IsAny<string>(),
                It.IsAny<Dictionary<string, dynamic>>(),
                It.IsAny<IEnumerable<Template>>())).Returns(
                new InsertDelegate((n, p, t) => 
                {
                    return Task.FromResult($"{n}: {string.Join("___", p.Keys.ToArray())}");
                }));

            var parser = new MarkdigMarkdownParser(paramsParserMock.Object);
            
            var res = await parser.Transform("abc\r\n{% include some value %}\r\nxyz", "", new ContextModel(null, null));

            //Assert.AreEqual("");
        }
    }
}
