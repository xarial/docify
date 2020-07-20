using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Loader
{
    public static class ILibraryLoaderExtension
    {
        /// <summary>
        /// Checks if the specified theme available in the library
        /// </summary>
        /// <param name="themeName">name of the theme</param>
        /// <returns>True if theme is available, False if not</returns>
        public static bool ContainsTheme(this ILibraryLoader libLoader, string themeName)
            => libLoader.Exists(new Location(Location.Library.ThemesFolderName, themeName, Enumerable.Empty<string>()));

        /// <summary>
        /// Checks if the specified component available in the library
        /// </summary>
        /// <param name="compName">Name of the component</param>
        /// <returns>True if component is available, False if not</returns>
        public static bool ContainsComponent(this ILibraryLoader libLoader, string compName)
            => libLoader.Exists(new Location(Location.Library.ComponentsFolderName, compName, Enumerable.Empty<string>()));

        /// <summary>
        /// Checks if the specified plugin available in the library
        /// </summary>
        /// <param name="pluginName">Name of the plugin</param>
        /// <returns>True if plugin is available, False if not</returns>
        public static bool ContainsPlugin(this ILibraryLoader libLoader, string pluginName)
            => libLoader.Exists(new Location(Location.Library.PluginsFolderName, pluginName, Enumerable.Empty<string>()));

        /// <summary>
        /// Loads the files of the theme
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <param name="filters">File filters</param>
        /// <returns>Files from the theme</returns>
        public static IAsyncEnumerable<IFile> LoadThemeFiles(this ILibraryLoader libLoader, string themeName, string[] filters)
            => libLoader.LoadFolder(new Location(Location.Library.ThemesFolderName, themeName, Enumerable.Empty<string>()), filters);

        /// <summary>
        /// Loads the files of the component
        /// </summary>
        /// <param name="componentName">Name of the component</param>
        /// <param name="filters">File filters</param>
        /// <returns>Files from the component</returns>
        public static IAsyncEnumerable<IFile> LoadComponentFiles(this ILibraryLoader libLoader, string componentName, string[] filters)
            => libLoader.LoadFolder(new Location(Location.Library.ComponentsFolderName, componentName, Enumerable.Empty<string>()), filters);

        /// <summary>
        /// Loads the files of the plugin
        /// </summary>
        /// <param name="pluginName">Name of the plugin</param>
        /// <param name="filters">File filters</param>
        /// <returns>Files from the plugin</returns>
        public static IAsyncEnumerable<IFile> LoadPluginFiles(this ILibraryLoader libLoader, string pluginName, string[] filters)
            => libLoader.LoadFolder(new Location(Location.Library.PluginsFolderName, pluginName, Enumerable.Empty<string>()), filters);
    }
}
