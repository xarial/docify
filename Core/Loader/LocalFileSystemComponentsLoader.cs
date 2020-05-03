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
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemComponentsLoader : IComponentsLoader
    {
        private readonly ILoader m_Loader;
        private readonly Configuration m_Config;

        public LocalFileSystemComponentsLoader(ILoader loader, Configuration conf)
        {
            m_Loader = loader;
            m_Config = conf;
        }

        public async Task<IEnumerable<IFile>> Load(IEnumerable<IFile> srcFiles)
        {
            if (srcFiles == null)
            {
                srcFiles = Enumerable.Empty<IFile>();
            }

            var resFiles = srcFiles.ToDictionary(f => f.Location.ToId(), f => f, StringComparer.CurrentCultureIgnoreCase);

            if (m_Config.Components?.Any() == true)
            {
                foreach (var comp in m_Config.Components)
                {
                    await AddFiles(resFiles, m_Config.ComponentsFolder.Combine(comp), comp, false);
                }
            }

            foreach (var theme in m_Config.ThemesHierarchy) 
            {
                await AddFiles(resFiles, m_Config.ThemesFolder.Combine(theme), theme, true);
            }

            return resFiles.Values;
        }

        private async Task AddFiles(Dictionary<string, IFile> srcFiles, ILocation loc, string fragName, bool allowInherit) 
        {
            var newSrcFiles = await m_Loader.Load(loc);

            if (newSrcFiles != null) 
            {
                foreach (var newSrcFile in newSrcFiles) 
                {
                    var id = newSrcFile.Location.ToId();

                    if (!srcFiles.ContainsKey(id))
                    {
                        srcFiles.Add(id, newSrcFile);
                    }
                    else 
                    {
                        if (!allowInherit)
                        {
                            throw new DuplicateComponentSourceFileException(fragName, id);
                        }
                    }
                }
            }
        }
    }
}
