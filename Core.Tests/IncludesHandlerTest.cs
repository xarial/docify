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

namespace Core.Tests
{
    public class IncludesHandlerTest
    {
        private IncludesHandler m_Handler;

        [SetUp]
        public void Setup() 
        {
            m_Handler = new IncludesHandler(null);
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
        public void Insert_SimpleParameters()
        {
        }

        [Test]
        public void Insert_MergedParameters()
        {
        }

        [Test]
        public void Insert_MissingIncludes()
        {
        }
    }
}
