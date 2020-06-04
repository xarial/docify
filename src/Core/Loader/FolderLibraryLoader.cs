//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class FolderLibraryLoader : ILibraryLoader
    {
        private readonly ILocation m_Location;
        private readonly IFileLoader m_FileLoader;
        
        public FolderLibraryLoader(ILocation location, IFileLoader fileLoader)
        {
            m_Location = location;
            m_FileLoader = fileLoader;
        }

        public IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters)
            => LoadLibraryItem(componentName, Location.Library.ComponentsFolderName, filters);

        public IAsyncEnumerable<IFile> LoadPluginFiles(string pluginName, string[] filters)
            => LoadLibraryItem(pluginName, Location.Library.PluginsFolderName, filters);

        public IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters)
            => LoadLibraryItem(themeName, Location.Library.ThemesFolderName, filters);

        public bool ContainsTheme(string themeName) 
            => ContainsLibraryItem(Location.Library.ThemesFolderName, themeName);

        public bool ContainsComponent(string compName)
            => ContainsLibraryItem(Location.Library.ComponentsFolderName, compName);

        public bool ContainsPlugin(string pluginName)
            => ContainsLibraryItem(Location.Library.PluginsFolderName, pluginName);

        private IAsyncEnumerable<IFile> LoadLibraryItem(string itemName, string subFolder, string[] filters)
        {
            try
            {
                var compsLoc = m_Location.Combine(new Location(new string[] { subFolder, itemName }));
                return m_FileLoader.LoadFolder(compsLoc, filters);
            }
            catch (Exception ex)
            {
                throw new LibraryItemLoadException(itemName, subFolder, ex);
            }
        }

        private bool ContainsLibraryItem(string itemType, string itemName) 
        {
            return m_FileLoader.Exists(m_Location.Combine(new Location("", itemType, itemName)));
        }
    }
}
