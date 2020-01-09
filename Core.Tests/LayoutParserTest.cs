//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Compiler;

namespace Core.Tests
{
    public class LayoutParserTest
    {
        [Test]
        public void ContainsPlaceholder() 
        {
            var parser = new LayoutParser();
            
            Assert.IsTrue(parser.ContainsPlaceholder("Hello {{ content }}"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{ content }}"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{ content }} AAA"));
            Assert.IsTrue(parser.ContainsPlaceholder("ABC {{ content }} aaa"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{    content }}"));
            Assert.IsTrue(parser.ContainsPlaceholder("Hello {{ content }}"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{ content  }}"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{ content}} AAA"));
            Assert.IsTrue(parser.ContainsPlaceholder("ABC {{content}} aaa"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{ content}} AAA\r\n{{ content }}"));
            Assert.IsTrue(parser.ContainsPlaceholder("{{content }}"));
            Assert.IsFalse(parser.ContainsPlaceholder("{content} Some Text"));
        }

        [Test]
        public void InsertContent() 
        {
            var parser = new LayoutParser();

            Assert.AreEqual("Hello __replacement__", parser.InsertContent("Hello {{ content }}", "__replacement__"));
            Assert.AreEqual("__replacement__", parser.InsertContent("{{ content }}", "__replacement__"));
            Assert.AreEqual("__replacement__ AAA", parser.InsertContent("{{ content }} AAA", "__replacement__"));
            Assert.AreEqual("ABC __replacement__ aaa", parser.InsertContent("ABC {{ content }} aaa", "__replacement__"));
            Assert.AreEqual("__replacement__", parser.InsertContent("{{    content }}", "__replacement__"));
            Assert.AreEqual("Hello __replacement__", parser.InsertContent("Hello {{ content }}", "__replacement__"));
            Assert.AreEqual("__replacement__", parser.InsertContent("{{ content  }}", "__replacement__"));
            Assert.AreEqual("__replacement__ AAA", parser.InsertContent("{{ content}} AAA", "__replacement__"));
            Assert.AreEqual("ABC __replacement__ aaa", parser.InsertContent("ABC {{content}} aaa", "__replacement__"));
            Assert.AreEqual("__replacement__ AAA\r\n__replacement__", parser.InsertContent("{{ content}} AAA\r\n{{ content }}", "__replacement__"));
            Assert.AreEqual("__replacement__", parser.InsertContent("{{content }}", "__replacement__"));
            Assert.AreEqual("{content} Some Text", parser.InsertContent("{content} Some Text", "__replacement__"));
        }
    }
}
