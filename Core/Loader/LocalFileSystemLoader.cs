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
    public class LocalFileSystemLoader : ILoader
    {
        private readonly LocalFileSystemLoaderConfig m_Config;
        private readonly System.IO.Abstractions.IFileSystem m_FileSystem;

        public LocalFileSystemLoader(LocalFileSystemLoaderConfig config)
            : this(config, new System.IO.Abstractions.FileSystem())
        {
        }

        public LocalFileSystemLoader(LocalFileSystemLoaderConfig config, System.IO.Abstractions.IFileSystem fileSystem)
        {
            m_Config = config;
            m_FileSystem = fileSystem;
        }

        public async Task<IEnumerable<IFile>> Load(ILocation location)
        {
            var files = new List<IFile>();
            
            var path = location.ToPath();

            if (!m_FileSystem.Directory.Exists(path)) 
            {
                throw new MissingLocationException(path);
            }

            foreach (var filePath in m_FileSystem.Directory.GetFiles(path, 
                "*.*", SearchOption.AllDirectories))
            {
                var relPath = Path.GetRelativePath(path, filePath);

                if (!PathMatcher.Matches(m_Config.Ignore, relPath))
                {
                    var loc = Location.FromPath(relPath);

                    var content = await m_FileSystem.File.ReadAllBytesAsync(filePath);
                    files.Add(new SourceFile(loc, content));
                }
            }

            return files;
        }
    }
}
