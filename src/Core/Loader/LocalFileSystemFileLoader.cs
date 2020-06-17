//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemFileLoader : IFileLoader
    {
        private readonly System.IO.Abstractions.IFileSystem m_FileSystem;
        private readonly ILogger m_Logger;

        public LocalFileSystemFileLoader(ILogger logger)
            : this(new System.IO.Abstractions.FileSystem(), logger)
        {
        }

        public LocalFileSystemFileLoader(System.IO.Abstractions.IFileSystem fileSystem, ILogger logger)
        {
            m_FileSystem = fileSystem;
            m_Logger = logger;
        }

        public async IAsyncEnumerable<ILocation> EnumSubFolders(ILocation location)
        {
            await Task.CompletedTask;

            foreach (var dir in m_FileSystem.Directory.EnumerateDirectories(
                location.ToPath(), "*.*", SearchOption.TopDirectoryOnly)) 
            {
                yield return Location.FromPath(dir);
            }
        }

        public bool Exists(ILocation location)
        {
            if (location.IsFile())
            {
                return m_FileSystem.File.Exists(location.ToPath());
            }
            else 
            {
                return m_FileSystem.Directory.Exists(location.ToPath());
            }
        }

        public async IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters)
        {
            var path = location.ToPath();

            if (!m_FileSystem.Directory.Exists(path))
            {
                throw new MissingLocationException(path);
            }

            foreach (var filePath in m_FileSystem.Directory.EnumerateFiles(path,
                "*.*", SearchOption.AllDirectories))
            {
                var relPath = Path.GetRelativePath(path, filePath);

                var loc = Location.FromPath(relPath);

                if (loc.Matches(filters))
                {
                    var content = await m_FileSystem.File.ReadAllBytesAsync(filePath);

                    m_Logger.LogInformation($"Loading '{filePath}'", true);

                    yield return new Data.File(
                        loc, content, Guid.NewGuid().ToString());
                }
            }
        }
    }
}
