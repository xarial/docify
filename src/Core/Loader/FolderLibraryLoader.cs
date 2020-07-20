//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class FolderLibraryLoader : IFileLoader
    {
        private readonly ILocation m_Location;
        private readonly IFileLoader m_FileLoader;
        
        public FolderLibraryLoader(ILocation location, IFileLoader fileLoader)
        {
            m_Location = location;
            m_FileLoader = fileLoader;
        }

        public async IAsyncEnumerable<ILocation> EnumSubFolders(ILocation location)
        {
            await foreach (var subFolderLoc in m_FileLoader.EnumSubFolders(m_Location.Combine(location))) 
            {
                yield return subFolderLoc.GetRelative(m_Location);
            }
        }

        public bool Exists(ILocation location)
            => m_FileLoader.Exists(m_Location.Combine(location));

        public IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters)
            => m_FileLoader.LoadFolder(m_Location.Combine(location), filters);
    }
}
