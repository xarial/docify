﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Plugin;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Loader
{
    public class ProjectLoader : IProjectLoader
    {
        private const string IGNORE_FILE_PARAM_NAME = "ignore";
        
        private readonly IFileLoader m_FileLoader;
        private readonly ILibraryLoader m_LibraryLoader;
        private readonly IConfiguration m_Config;

        private readonly IPluginsManager m_PluginsManager;

        private readonly string[] m_Filter;
        private readonly ILoaderExtension m_Ext;
        private readonly ILogger m_Logger;

        public ProjectLoader(IFileLoader fileLoader,
            ILibraryLoader libraryLoader, IPluginsManager pluginsMgr, 
            IConfiguration conf, ILoaderExtension ext,
            ILogger logger)
        {
            m_FileLoader = fileLoader;
            m_LibraryLoader = libraryLoader;
            m_Config = conf;
            m_PluginsManager = pluginsMgr;

            m_Ext = ext;
            m_Logger = logger;

            m_Filter = GetFilesFilter();
        }

        public async IAsyncEnumerable<IFile> Load(ILocation[] locations)
        {
            var resFileIds = new List<string>();

            await m_PluginsManager.LoadPlugins(LoadPluginFiles(locations, resFileIds));

            foreach (var loc in locations)
            {
                m_Logger.LogInformation($"Loading project files from {loc.ToId()}");

                var filter = new List<string>();

                if (m_Filter != null)
                {
                    filter.AddRange(m_Filter);
                }

                var pluginExcludeFilter = LocationExtension.NEGATIVE_FILTER
                    + Path.Combine(Location.Library.PluginsFolderName, LocationExtension.ANY_FILTER);

                filter.Add(pluginExcludeFilter);

                await foreach (var srcFile in m_FileLoader.LoadFolder(loc, filter.ToArray()))
                {
                    var args = new PreLoadFileArgs()
                    {
                        File = srcFile,
                        SkipFile = false
                    };

                    await m_Ext.PreLoadFile(args);

                    if (!args.SkipFile)
                    {
                        var id = args.File.Location.ToId();

                        if (!resFileIds.Contains(id))
                        {
                            resFileIds.Add(id);
                            yield return args.File;
                        }
                        else
                        {
                            throw new DuplicateFileException(id, loc.ToId());
                        }
                    }
                }
            }

            if (m_Config.Components?.Any() == true)
            {
                foreach (var compName in m_Config.Components)
                {
                    await foreach (var srcFile in ProcessLibraryItems(
                            m_LibraryLoader.LoadComponentFiles(compName, m_Filter), resFileIds, false))
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
                            m_LibraryLoader.LoadThemeFiles(themeName, m_Filter), resFileIds, true))
                        {
                            yield return srcFile;
                        }
                    }
                }
            }
        }
        
        private async IAsyncEnumerable<IPluginInfo> LoadPluginFiles(ILocation[] locations, List<string> resFileIds)
        {
            foreach (var loc in locations)
            {
                var pluginsLoc = loc.Combine(Location.Library.PluginsFolderName);

                if (m_FileLoader.Exists(pluginsLoc))
                {
                    await foreach (var pluginLoc in m_FileLoader.EnumSubFolders(pluginsLoc))
                    {
                        yield return new PluginInfo(pluginLoc.Segments.Last(), m_FileLoader.LoadFolder(pluginLoc, m_Filter));
                    }
                }
            }

            if (m_Config.Plugins?.Any() == true)
            {
                foreach (var pluginName in m_Config.Plugins)
                {
                    yield return new PluginInfo(pluginName, ProcessLibraryItems(
                        m_LibraryLoader.LoadPluginFiles(pluginName, null), resFileIds, true));
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
