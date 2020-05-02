//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Composition;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin;

namespace Xarial.Docify.Core.Publisher
{
    public class LocalFileSystemPublisher : IPublisher
    {
        private readonly LocalFileSystemPublisherConfig m_Config;
        private readonly IFileSystem m_FileSystem;

        [ImportPlugin]
        private IEnumerable<IPrePublishAssetPlugin> m_PrePublishAssetPlugins = null;
        
        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config) 
            : this(config, new FileSystem())
        {
        }

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config, IFileSystem fileSystem)
        {
            m_Config = config;
            m_FileSystem = fileSystem;
        }

        public async Task Write(Location loc, IEnumerable<IWritable> writables)
        {
            var outDir = loc.ToPath();

            if (m_FileSystem.Directory.Exists(outDir))
            {
                m_FileSystem.Directory.Delete(outDir, true);
            }

            foreach (var writable in writables)
            {
                var outFilePath = writable.Location.ToPath();

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

                bool cancel = false;

                var binContent = writable.Content;

                m_PrePublishAssetPlugins.InvokePluginsIfAny(p => p.PrePublishAsset(ref outLoc, ref binContent, out cancel));
                if (!cancel)
                {
                    outFilePath = outLoc.ToPath();
                    CreateDirectoryIfNeeded();
                    await m_FileSystem.File.WriteAllBytesAsync(outFilePath, binContent);
                }
            }
        }
    }
}
