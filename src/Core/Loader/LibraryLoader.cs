//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LibraryLoader : ILibraryLoader
    {
        private readonly IFileLoader[] m_Loaders;

        public LibraryLoader(IFileLoader[] loaders) 
        {
            m_Loaders = loaders;
        }

        public IAsyncEnumerable<ILocation> EnumSubFolders(ILocation location) 
            => FindLoader(location).EnumSubFolders(location);

        public bool Exists(ILocation location)
            => m_Loaders.Any(l => l.Exists(location));

        public IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters)
            => FindLoader(location).LoadFolder(location, filters);

        private IFileLoader FindLoader(ILocation loc) 
        {
            var loader = m_Loaders.FirstOrDefault(l => l.Exists(loc));

            if (loader != null)
            {
                return loader;
            }
            else
            {
                throw new LibraryItemLoadException(loc);
            }
        }
    }
}
