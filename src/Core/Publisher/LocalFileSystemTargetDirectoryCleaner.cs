//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.IO.Abstractions;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Publisher
{
    public class LocalFileSystemTargetDirectoryCleaner : ITargetDirectoryCleaner
    {
        private readonly IFileSystem m_FileSystem;
        private readonly bool m_ClearTarget;

        public LocalFileSystemTargetDirectoryCleaner() : this(new FileSystem(), true)
        {
        }

        public LocalFileSystemTargetDirectoryCleaner(IFileSystem fileSystem,
            bool clearTarget) 
        {
            m_FileSystem = fileSystem;
            m_ClearTarget = clearTarget;
        }

        public Task ClearDirectory(ILocation outDir)
        {
            var outDirPath = outDir.ToPath();

            if (m_ClearTarget)
            {
                if (m_FileSystem.Directory.Exists(outDirPath))
                {
                    m_FileSystem.Directory.Delete(outDirPath, true);
                }
            }

            return Task.CompletedTask;
        }
    }
}
