//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemFragmentsLoader : IFragmentsLoader
    {
        private readonly IFileSystem m_FileSystem;

        public LocalFileSystemFragmentsLoader() : this (new FileSystem())
        {
        }

        public LocalFileSystemFragmentsLoader(IFileSystem fs) 
        {
            m_FileSystem = fs;
        }

        public IEnumerable<ISourceFile> GetFiles(Location fragmentsLoc, params string[] fragments)
        {
            var path = fragmentsLoc.ToPath();

            foreach (var fragment in fragments) 
            {
                var fragmentDir = Path.Combine(path, fragment);

                if (!m_FileSystem.Directory.Exists(fragmentDir)) 
                {
                    throw new MissingFragmentException(fragment, fragmentDir);
                }

                foreach (var file in m_FileSystem.Directory.GetFiles(fragmentDir)) 
                {
                     
                }
            }

            return null;
        }
    }
}
