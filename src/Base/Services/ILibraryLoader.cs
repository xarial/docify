//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    /// <summary>
    /// Service loads library items
    /// </summary>
    public interface ILibraryLoader
    {
        /// <summary>
        /// Checks if the specified theme available in the library
        /// </summary>
        /// <param name="themeName">name of the theme</param>
        /// <returns>True if theme is available, False if not</returns>
        bool ContainsTheme(string themeName);

        /// <summary>
        /// Checks if the specified component available in the library
        /// </summary>
        /// <param name="compName">Name of the component</param>
        /// <returns>True if component is available, False if not</returns>
        bool ContainsComponent(string compName);

        /// <summary>
        /// Checks if the specified plugin available in the library
        /// </summary>
        /// <param name="pluginId">Id of the plugin</param>
        /// <returns>True if plugin is available, False if not</returns>
        bool ContainsPlugin(string pluginId);

        /// <summary>
        /// Loads the files of the theme
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <param name="filters">File filters</param>
        /// <returns>Files from the theme</returns>
        IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters);

        /// <summary>
        /// Loads the files of the component
        /// </summary>
        /// <param name="componentName">Name of the component</param>
        /// <param name="filters">File filters</param>
        /// <returns>Files from the component</returns>
        IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters);

        /// <summary>
        /// Loads the files of the plugin
        /// </summary>
        /// <param name="pluginId">ID of the plugin</param>
        /// <param name="filters">File filters</param>
        /// <returns>Files from the plugin</returns>
        IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters);
    }
}
