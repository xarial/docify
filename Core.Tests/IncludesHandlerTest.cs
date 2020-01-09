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
        }

        [Test]
        public void ParseParameters_MultipleLine()
        {
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
