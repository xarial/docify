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
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemComponentsLoader : IComponentsLoader
    {
        private readonly ILoader m_Loader;
        private readonly IConfiguration m_Config;

        public LocalFileSystemComponentsLoader(ILoader loader, IConfiguration conf)
        {
            m_Loader = loader;
            m_Config = conf;
        }

        public async IAsyncEnumerable<IFile> Load(IAsyncEnumerable<IFile> srcFiles)
        {
            var resFileIds = new List<string>();

            await foreach (var srcFile in srcFiles) 
            {
                resFileIds.Add(srcFile.Location.ToId());
                yield return srcFile;
            }
            
            if (m_Config.Components?.Any() == true)
            {
                foreach (var comp in m_Config.Components)
                {
                    await foreach (var file in LoadFiles(resFileIds, m_Config.ComponentsFolder.Combine(comp), comp, false)) 
                    {
                        yield return file;
                    }
                }
            }

            foreach (var theme in m_Config.ThemesHierarchy) 
            {
                await foreach (var file in LoadFiles(resFileIds, m_Config.ThemesFolder.Combine(theme), theme, true)) 
                {
                    yield return file;
                }
            }
        }

        private async IAsyncEnumerable<IFile> LoadFiles(
            List<string> resFileIds, ILocation loc, string fragName, bool allowInherit) 
        {
            await foreach (var newSrcFile in m_Loader.Load(new ILocation[] { loc }))
            {
                var id = newSrcFile.Location.ToId();

                if (!resFileIds.Contains(id))
                {
                    resFileIds.Add(id);
                    yield return newSrcFile;
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
