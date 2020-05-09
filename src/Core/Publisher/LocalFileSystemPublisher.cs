﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Plugin;

namespace Xarial.Docify.Core.Publisher
{
    public class LocalFileSystemPublisher : IPublisher
    {
        private readonly LocalFileSystemPublisherConfig m_Config;
        private readonly System.IO.Abstractions.IFileSystem m_FileSystem;

        [ImportPlugins]
        private IEnumerable<IPrePublishFilePlugin> m_PrePublishFilePlugins = null;

        [ImportPlugins]
        private IEnumerable<IPostPublishPlugin> m_PostPublishPlugins = null;

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config) 
            : this(config, new System.IO.Abstractions.FileSystem())
        {
        }

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config, System.IO.Abstractions.IFileSystem fileSystem)
        {
            m_Config = config;
            m_FileSystem = fileSystem;
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
                
                void CreateDirectoryIfNeeded()
                {
                    var dir = Path.GetDirectoryName(outFilePath);

                    if (!m_FileSystem.Directory.Exists(dir))
                    {
                        m_FileSystem.Directory.CreateDirectory(dir);
                    }
                }

                bool skip = false;

                IFile outFile = new Data.File(outLoc, file.Content, file.Id);
                
                await m_PrePublishFilePlugins.InvokePluginsIfAnyAsync(async (p) =>
                {
                    var res = await p.PrePublishFile(loc, outFile);

                    if (res.SkipFile)
                    {
                        skip = true;
                    }
                    else 
                    {
                        outFile = res.File;
                    }
                });

                if (!skip)
                {
                    outFilePath = outFile.Location.ToPath();
                    CreateDirectoryIfNeeded();
                    await m_FileSystem.File.WriteAllBytesAsync(outFilePath, outFile.Content);
                }
            }

            await m_PostPublishPlugins.InvokePluginsIfAnyAsync(async (p) => await p.PostPublish(loc));
        }
    }
}