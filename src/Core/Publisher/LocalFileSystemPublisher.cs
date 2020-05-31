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
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Publisher
{
    public class LocalFileSystemPublisher : IPublisher
    {
        private readonly System.IO.Abstractions.IFileSystem m_FileSystem;

        private readonly IPublisherExtension m_Ext;

        public LocalFileSystemPublisher(IPublisherExtension ext)
            : this(new System.IO.Abstractions.FileSystem(), ext)
        {
        }

        public LocalFileSystemPublisher(
            System.IO.Abstractions.IFileSystem fileSystem, IPublisherExtension ext)
        {
            m_FileSystem = fileSystem;
            m_Ext = ext;
        }

        public async Task Write(ILocation loc, IAsyncEnumerable<IFile> files)
        {
            var outDir = loc.ToPath();

            if (m_FileSystem.Directory.Exists(outDir))
            {
                m_FileSystem.Directory.Delete(outDir, true);
            }

            await foreach (var file in files)
            {
                var outFilePath = file.Location.ToPath();

                if (!Path.IsPathRooted(outFilePath))
                {
                    outFilePath = Path.Combine(outDir, outFilePath);
                }

                var outLoc = Location.FromPath(outFilePath);

                IFile outFile = new Data.File(outLoc, file.Content, file.Id);

                var res = await m_Ext.PrePublishFile(loc, outFile);

                if (!res.SkipFile)
                {
                    await WriteFile(res.File);
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

            await m_FileSystem.File.WriteAllBytesAsync(outFilePath, file.Content);
        }
    }
}
