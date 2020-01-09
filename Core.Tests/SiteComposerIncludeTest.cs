//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Base;
using Xarial.Docify.Core.Exceptions;
using Moq;

namespace Core.Tests
{
    public class SiteComposerIncludeTest
    {
        private SiteComposer m_Composer;

        [SetUp]
        public void Setup()
        {
            m_Composer = new SiteComposer(new Mock<ILayoutParser>().Object);
        }

        [Test]
        public void ComposeSite_SingleInclude() 
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_includes\\i1.md"), "Include"),
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.Includes.Count);
            Assert.AreEqual("i1", site.Includes[0].Name);
            Assert.AreEqual(0, site.Includes[0].Data.Count);
            Assert.AreEqual("Include", site.Includes[0].RawContent);
        }

        [Test]
        public void ComposeSite_MultipleInclude()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_includes\\i1.md"), "i1content"),
                new TextSourceFile(Location.FromPath(@"_includes\\i2.txt"), "i2content"),
                new TextSourceFile(Location.FromPath(@"_includes\\i3.ini"), "i3content"),
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(3, site.Includes.Count);
            Assert.AreEqual(0, site.Includes.Select(i => i.Name).Except(new string[] { "i1", "i2", "i3" }).Count());
            Assert.AreEqual("i1content", site.Includes.First(i => i.Name == "i1").RawContent);
            Assert.AreEqual("i2content", site.Includes.First(i => i.Name == "i2").RawContent);
            Assert.AreEqual("i3content", site.Includes.First(i => i.Name == "i3").RawContent);
        }

        [Test]
        public void ComposeSite_IncludeMetadata()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_includes\\i1.md"), "---\r\nprp1: A\r\nprp2: B\r\n---\r\ni1content"),
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
            };

            var site = m_Composer.ComposeSite(src, "");

            Assert.AreEqual(1, site.Includes.Count);
            Assert.AreEqual("i1", site.Includes[0].Name);
            Assert.AreEqual("i1content", site.Includes[0].RawContent);
            Assert.AreEqual("A", site.Includes[0].Data["prp1"]);
            Assert.AreEqual("B", site.Includes[0].Data["prp2"]);
        }

        [Test]
        public void ComposeSite_DuplicateIncludes()
        {
            var src = new TextSourceFile[]
            {
                new TextSourceFile(Location.FromPath(@"_includes\\i1.md"), ""),
                new TextSourceFile(Location.FromPath(@"_includes\\i1.txt"), ""),
                new TextSourceFile(Location.FromPath(@"index.md"), ""),
            };

            Assert.Throws<DuplicateTemplateException>(() => m_Composer.ComposeSite(src, ""));
        }
    }
};