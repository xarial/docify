//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class ProjectLoader : IProjectLoader
    {
        private const string IGNORE_FILE_PARAM_NAME = "ignore";
        private const string PLUGINS_FOLDER = "_plugins";

        private readonly IFileLoader m_FileLoader;
        private readonly ILibraryLoader m_LibraryLoader;
        private readonly IConfiguration m_Config;

        private readonly IPluginsManager m_PluginsManager;

        private readonly string[] m_Filter;

        public ProjectLoader(IFileLoader fileLoader,
            ILibraryLoader libraryLoader, IPluginsManager pluginsMgr, IConfiguration conf)
        {
            m_FileLoader = fileLoader;
            m_LibraryLoader = libraryLoader;
            m_Config = conf;
            m_PluginsManager = pluginsMgr;

            m_Filter = GetFilesFilter();
        }

        public async IAsyncEnumerable<IFile> Load(ILocation[] locations)
        {
            var resFileIds = new List<string>();

            await m_PluginsManager.LoadPlugins(LoadPluginFiles(locations, resFileIds));

            foreach (var loc in locations)
            {
                var pluginFilter = GetPluginsFolderFilter(loc);

                var filter = new List<string>();

                if (m_Filter != null)
                {
                    filter.AddRange(m_Filter);
                }

                filter.Add(LocationExtension.NEGATIVE_FILTER + pluginFilter);

                await foreach (var srcFile in m_FileLoader.LoadFolder(loc, filter.ToArray()))
                {
                    var id = srcFile.Location.ToId();

                    if (!resFileIds.Contains(id))
                    {
                        resFileIds.Add(id);
                        yield return srcFile;
                    }
                    else
                    {
                        throw new DuplicateFileException(id, loc.ToId());
                    }
                }
            }

            if (m_Config.Components?.Any() == true)
            {
                foreach (var compName in m_Config.Components)
                {
                    await foreach (var srcFile in ProcessLibraryItems(
                            m_LibraryLoader.LoadComponentFiles(compName, null), resFileIds, false))
                    {
                        yield return srcFile;
                    }
                }
            }

            if (m_Config.ThemesHierarchy?.Any() == true)
            {
                foreach (var theme in m_Config.ThemesHierarchy)
                {
                    foreach (var themeName in m_Config.ThemesHierarchy)
                    {
                        await foreach (var srcFile in ProcessLibraryItems(
                            m_LibraryLoader.LoadThemeFiles(themeName, null), resFileIds, true))
                        {
                            yield return srcFile;
                        }
                    }
                }
            }
        }

        private string GetPluginsFolderFilter(ILocation baseLoc)
        {
            return baseLoc.ToId() + LocationExtension.ID_SEP
                    + PLUGINS_FOLDER + LocationExtension.ID_SEP + LocationExtension.ANY_FILTER;
        }

        private async IAsyncEnumerable<IFile> LoadPluginFiles(ILocation[] locations, List<string> resFileIds)
        {
            foreach (var loc in locations)
            {
                var pluginFilter = GetPluginsFolderFilter(loc);

                await foreach (var srcFile in m_FileLoader.LoadFolder(loc, new string[] { pluginFilter }))
                {
                    var id = srcFile.Location.ToId();

                    if (!resFileIds.Contains(id))
                    {
                        resFileIds.Add(id);
                        yield return srcFile;
                    }
                    else
                    {
                        throw new DuplicateFileException(id, loc.ToId());
                    }
                }
            }

            if (m_Config.Plugins?.Any() == true)
            {
                foreach (var pluginId in m_Config.Plugins)
                {
                    await foreach (var srcFile in ProcessLibraryItems(
                        m_LibraryLoader.LoadPluginFiles(pluginId, null), resFileIds, true))
                    {
                        yield return srcFile;
                    }
                }
            }
        }

        private async IAsyncEnumerable<IFile> ProcessLibraryItems(
            IAsyncEnumerable<IFile> libFiles, List<string> resFileIds, bool allowInherit)
        {
            await foreach (var newSrcFile in libFiles)
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
                        throw new DuplicateComponentSourceFileException(id);
                    }
                }
            }
        }

        private string[] GetFilesFilter()
        {
            try
            {
                var vals = m_Config.GetParameterOrDefault<IEnumerable<string>>(IGNORE_FILE_PARAM_NAME);

                if (vals != null)
                {
                    return vals.Select(f => LocationExtension.RevertFilter(f)).ToArray();
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw new InvalidCastException($"Value specified in {IGNORE_FILE_PARAM_NAME} must be an array");
            }
        }
    }
}
