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

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var libLoader = new LibraryLoader(Location.FromPath("D:\\lib"), fileLoaderMock.Object);
            libLoader.LoadComponentFiles("A", null);

            Assert.AreEqual("D:\\lib\\_components\\A", loc.ToPath());
        }

        [Test]
        public void LoadThemeFilesTest() 
        {
            ILocation loc = null;

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var libLoader = new LibraryLoader(Location.FromPath("D:\\lib"), fileLoaderMock.Object);
            libLoader.LoadThemeFiles("A", null);

            Assert.AreEqual("D:\\lib\\_themes\\A", loc.ToPath());
        }

        [Test]
        public void LoadPluginFilesTest() 
        {
            ILocation loc = null;

            var fileLoaderMock = new Mock<IFileLoader>();
            fileLoaderMock.Setup(m => m.LoadFolder(It.IsAny<ILocation>(), It.IsAny<string[]>()))
                .Returns((ILocation l, string[] f) =>
                {
                    loc = l;
                    return AsyncEnumerable.Empty<IFile>();
                });

            var libLoader = new LibraryLoader(Location.FromPath("D:\\lib"), fileLoaderMock.Object);
            libLoader.LoadPluginFiles("A", null);

            Assert.AreEqual("D:\\lib\\_plugins\\A", loc.ToPath());
        }
    }
}
