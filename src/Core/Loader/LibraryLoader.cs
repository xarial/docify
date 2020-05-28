using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LibraryLoader : ILibraryLoader
    {
        private const string COMPONENTS_FOLDER = "_components";
        private const string THEMES_FOLDER = "_themes";
        private const string PLUGINS_FOLDER = "_plugins";

        private readonly ILocation m_Location;
        private readonly IFileLoader m_FileLoader;

        public LibraryLoader(ILocation location, IFileLoader fileLoader) 
        {
            m_Location = location;
            m_FileLoader = fileLoader;
        }

        public IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters = null)
            => LoadLibraryItem(componentName, COMPONENTS_FOLDER, filters);

        public IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters = null)
            => LoadLibraryItem(pluginId, PLUGINS_FOLDER, filters);

        public IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters = null)
            => LoadLibraryItem(themeName, THEMES_FOLDER, filters);

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
    }
}
