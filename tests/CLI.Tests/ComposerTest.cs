using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Common.Mocks;
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
            m_Composer = new DocifyEngineMock().Resove<IComposer>();
        }

        [Test]
        public async Task SinglePageTest()
        {
            var files = new IFile[]
            {
                new FileMock(Location.FromPath("index.html"), "test")
            };

            var site = await m_Composer.ComposeSite(files.ToAsyncEnumerable(), "");

            Assert.AreEqual(0, site.MainPage.SubPages.Count);
            Assert.AreEqual("test", site.MainPage.RawContent);
        }
    }
}
