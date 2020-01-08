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
using Xarial.Docify.Core.Base;

namespace Core.Tests
{
    public class LocationTest
    {
        [Test]
        public void FromPathTest() 
        {
            var r1 = Location.FromPath(@"index.md", @"C:\MySite");
            var r2 = Location.FromPath(@"C:\MySite\page1\index.md", @"C:\MySite");
            var r3 = Location.FromPath(@"page2\index.md", @"C:\MySite");
            var r4 = Location.FromPath(@"\page3\subpage3\index.md", @"C:\MySite");

            Assert.AreEqual("index.md", r1.ToId());
            Assert.AreEqual("page1-index.md", r2.ToId());
            Assert.AreEqual("page2-index.md", r3.ToId());
            Assert.AreEqual("page3-subpage3-index.md", r4.ToId());
        }
    }
}
