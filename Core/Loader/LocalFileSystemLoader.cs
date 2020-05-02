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
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemLoader : ILoader
    {
        private readonly LocalFileSystemLoaderConfig m_Config;
        private readonly IFileSystem m_FileSystem;

        public LocalFileSystemLoader(LocalFileSystemLoaderConfig config)
            : this(config, new FileSystem())
        {
        }

        public LocalFileSystemLoader(LocalFileSystemLoaderConfig config, IFileSystem fileSystem)
        {
            m_Config = config;
            m_FileSystem = fileSystem;
        }

        public async Task<IEnumerable<ISourceFile>> Load(Location location)
        {
            var files = new List<ISourceFile>();

            //TODO: combine into single regex
            var ignoreRegex = m_Config.Ignore.Select(
                i => "^" + Regex.Escape(i).Replace("\\*", ".*").Replace("\\?", ".") + "$").ToArray();

            var path = location.ToPath();

            if (!m_FileSystem.Directory.Exists(path)) 
            {
                throw new MissingLocationException(path);
            }

            foreach (var filePath in m_FileSystem.Directory.GetFiles(path, 
                "*.*", SearchOption.AllDirectories))
            {
                var relPath = Path.GetRelativePath(path, filePath);

                if (!ignoreRegex.Any(i => Regex.IsMatch(relPath, i, RegexOptions.IgnoreCase)))
                {
                    var loc = Location.FromPath(relPath);

                    if (IsTextFile(filePath))
                    {
                        var content = await m_FileSystem.File.ReadAllTextAsync(filePath);
                        files.Add(new TextSourceFile(loc, content));
                    }
                    else
                    {
                        var content = await m_FileSystem.File.ReadAllBytesAsync(filePath);
                        files.Add(new BinarySourceFile(loc, content));
                    }
                }
            }

            return files;
        }

        private bool IsTextFile(string filePath) 
        {
            var ext = Path.GetExtension(filePath).TrimStart('.');

            return m_Config.TextFileExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}
