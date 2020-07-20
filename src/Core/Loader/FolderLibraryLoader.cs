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
    public class FolderLibraryLoader : LocalFileSystemFileLoader, ILibraryLoader
    {
        private readonly ILocation m_Location;
        private readonly IFileLoader m_FileLoader;
        
        public FolderLibraryLoader(ILocation location, IFileLoader fileLoader) : base(null, null)
        {
            m_Location = location;
            m_FileLoader = fileLoader;
        }
    }
}
