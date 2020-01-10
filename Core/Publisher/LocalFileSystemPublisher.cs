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
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Publisher
{
    public class LocalFileSystemPublisher : IPublisher
    {
        private readonly LocalFileSystemPublisherConfig m_Config;
        private readonly IFileSystem m_FileSystem;

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config) :
            this(config, new FileSystem())
        {
        }

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config, IFileSystem fileSystem)
        {
            m_Config = config;
            m_FileSystem = fileSystem;
        }

        public async Task Write(IEnumerable<IWritable> writables)
        {
            if (m_FileSystem.Directory.Exists(m_Config.OutDir))
            {
                m_FileSystem.Directory.Delete(m_Config.OutDir, true);
            }

            foreach (var writable in writables)
            {
                var outFilePath = writable.Location.ToPath();

                if (!Path.IsPathRooted(outFilePath)) 
                {
                    outFilePath = Path.Combine(m_Config.OutDir, outFilePath);
                }
                
                var dir = Path.GetDirectoryName(outFilePath);

                if (!m_FileSystem.Directory.Exists(dir))
                {
                    m_FileSystem.Directory.CreateDirectory(dir);
                }

                switch (writable)
                {
                    case ITextWritable text:
                        await m_FileSystem.File.WriteAllTextAsync(outFilePath, text.Content);
                        break;

                    case IBinaryWritable bin:
                        await m_FileSystem.File.WriteAllBytesAsync(outFilePath, bin.Content);
                        break;

                    default:
                        throw new NotSupportedException();
                }
            }
        }
    }
}
