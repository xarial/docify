//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Helpers;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemFileLoader : IFileLoader
    {
        private readonly System.IO.Abstractions.IFileSystem m_FileSystem;

        public LocalFileSystemFileLoader()
            : this(new System.IO.Abstractions.FileSystem())
        {
        }

        public LocalFileSystemFileLoader(System.IO.Abstractions.IFileSystem fileSystem)
        {
            m_FileSystem = fileSystem;
        }

        public async IAsyncEnumerable<IFile> LoadFolder(ILocation location, 
            string[] filters = null)
        {
            if (filters != null) 
            {
                filters = filters.Select(f => f
                    .Replace(LocationExtension.PATH_SEP, LocationExtension.ID_SEP)
                    .Replace(LocationExtension.URL_SEP, LocationExtension.ID_SEP)).ToArray();
            }

            if (location.IsFile()) 
            {
                throw new Exception("Specified location is not a folder");
            }
            
            var path = location.ToPath();

            if (!m_FileSystem.Directory.Exists(path))
            {
                throw new MissingLocationException(path);
            }

            foreach (var filePath in m_FileSystem.Directory.GetFiles(path,
                "*.*", SearchOption.AllDirectories))
            {
                var relPath = Path.GetRelativePath(path, filePath);

                if (PathMatcher.Matches(filters, relPath))
                {
                    var loc = Location.FromPath(relPath);
                    yield return await LoadFile(loc);
                }
            }
        }

        public async Task<IFile> LoadFile(ILocation location)
        {
            if (!location.IsFile()) 
            {
                throw new Exception("Specified location is not a file");
            }

            var path = location.ToPath();

            if (!m_FileSystem.File.Exists(path))
            {
                throw new MissingLocationException(path);
            }

            var content = await m_FileSystem.File.ReadAllBytesAsync(path);
            return new Data.File(location, content, Guid.NewGuid().ToString());
        }
    }
}
