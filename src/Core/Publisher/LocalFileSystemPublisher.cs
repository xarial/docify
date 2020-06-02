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
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Publisher
{
    public class LocalFileSystemPublisher : IPublisher
    {
        private readonly System.IO.Abstractions.IFileSystem m_FileSystem;

        private readonly IPublisherExtension m_Ext;
        private readonly ITargetDirectoryCleaner m_TargetCleaner;
        
        public LocalFileSystemPublisher(IPublisherExtension ext)
            : this(new System.IO.Abstractions.FileSystem(), ext, 
                  new LocalFileSystemTargetDirectoryCleaner(new System.IO.Abstractions.FileSystem(), true))
        {
        }

        public LocalFileSystemPublisher(IPublisherExtension ext,
            ITargetDirectoryCleaner targetCleaner)
            : this(new System.IO.Abstractions.FileSystem(), ext, targetCleaner)
        {
        }

        public LocalFileSystemPublisher(
            System.IO.Abstractions.IFileSystem fileSystem, IPublisherExtension ext,
            ITargetDirectoryCleaner targetCleaner)
        {
            m_FileSystem = fileSystem;
            m_Ext = ext;

            m_TargetCleaner = targetCleaner;
        }

        public async Task Write(ILocation loc, IAsyncEnumerable<IFile> files)
        {
            var outDir = loc.ToPath();

            await m_TargetCleaner.ClearDirectory(loc);
            
            await foreach (var file in files)
            {
                var outFilePath = file.Location.ToPath();

                if (!Path.IsPathRooted(outFilePath))
                {
                    outFilePath = Path.Combine(outDir, outFilePath);
                }

                var outLoc = Location.FromPath(outFilePath);

                var args = new PrePublishFileArgs()
                {
                    File = new Data.File(outLoc, file.Content, file.Id),
                    SkipFile = false
                };

                await m_Ext.PrePublishFile(loc, args);

                if (!args.SkipFile)
                {
                    await WriteFile(args.File);
                }
            }

            var additionalFiles = m_Ext.PostAddPublishFiles(loc);

            if (additionalFiles != null)
            {
                await foreach (var addFile in additionalFiles)
                {
                    await WriteFile(addFile);
                }
            }

            await m_Ext.PostPublish(loc);
        }

        private async Task WriteFile(IFile file)
        {
            var outFilePath = file.Location.ToPath();

            if (!Path.IsPathRooted(outFilePath))
            {
                throw new Exception($"Path of file to publish {outFilePath} must be rooted");
            }

            var dir = Path.GetDirectoryName(outFilePath);

            if (!m_FileSystem.Directory.Exists(dir))
            {
                m_FileSystem.Directory.CreateDirectory(dir);
            }

            if (m_FileSystem.File.Exists(outFilePath))
            {
                throw new FilePublishOverwriteForbiddenException(outFilePath);
            }

            await m_FileSystem.File.WriteAllBytesAsync(outFilePath, file.Content);
        }
    }
}
