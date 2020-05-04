using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;

namespace CLI.Tests
{
    class ComposerTest
    {
        private IComposer m_Composer;

        [SetUp]
        public void Setup()
        {
            m_Composer = new DocifyEngineMock(@"D:\src", @"D:\out", "www.xarial.com", Environment_e.Test).Resove<IComposer>();
        }

        [Test]
        public async Task SinglePageTest()
        {
            var files = new IFile[]
            {
                new SourceFile(Location.FromPath("index.html"), "test")
            };

            var site = m_Composer.ComposeSite(files, "");

            Assert.AreEqual(1, site.GetAllPages().Count());
            Assert.AreEqual("test", site.MainPage.RawContent);
        }
    }
}
