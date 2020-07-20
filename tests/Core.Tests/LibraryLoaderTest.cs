//*********************************************************************
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Loader;

namespace Core.Tests
{
    public class LibraryLoaderTest
    {
        [Test]
        public void LoadComponentFilesTest() 
        {
            ILocation loc = null;

            var fileLoaderMock = new Mock<ILibraryLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var libLoader = fileLoaderMock.Object;
            libLoader.LoadComponentFiles("A", null);

            Assert.AreEqual("_components::A", loc.ToId());
        }

        [Test]
        public void LoadThemeFilesTest() 
        {
            ILocation loc = null;

            var fileLoaderMock = new Mock<ILibraryLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var libLoader = fileLoaderMock.Object;
            libLoader.LoadThemeFiles("A", null);

            Assert.AreEqual("_themes\\A", loc.ToPath());
        }

        [Test]
        public void LoadPluginFilesTest() 
        {
            ILocation loc = null;

            var fileLoaderMock = new Mock<ILibraryLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var libLoader = fileLoaderMock.Object;
            libLoader.LoadPluginFiles("A", null);

            Assert.AreEqual("_plugins\\A", loc.ToPath());
        }

        [Test]
        public void ContainsTest() 
        {
            var lib = new List<string>();
            lib.Add("_components::c1");
            lib.Add("_themes::t1");
            lib.Add("_plugins::p1");

            var fileLoaderMock = new Mock<ILibraryLoader>();
            fileLoaderMock.Setup(m => m.Exists(It.IsAny<ILocation>()))
                .Returns((ILocation l) => lib.Contains(l.ToId()));

            var libLoader = fileLoaderMock.Object;

            var r1 = libLoader.ContainsComponent("c1");
            var r2 = libLoader.ContainsComponent("c2");
            var r3 = libLoader.ContainsTheme("t1");
            var r4 = libLoader.ContainsTheme("t2");
            var r5 = libLoader.ContainsPlugin("p1");
            var r6 = libLoader.ContainsPlugin("p2");

            Assert.IsTrue(r1);
            Assert.IsFalse(r2);
            Assert.IsTrue(r3);
            Assert.IsFalse(r4);
            Assert.IsTrue(r5);
            Assert.IsFalse(r6);
        }
    }
}
