using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class LibraryLoader : ILibraryLoader
    {
        private readonly ILibraryLoader[] m_Loaders;

        public LibraryLoader(ILibraryLoader[] loaders) 
        {
            m_Loaders = loaders;
        }

        public bool ContainsComponent(string compName) 
            => m_Loaders.Any(l => l.ContainsComponent(compName));

        public bool ContainsPlugin(string pluginId)
            => m_Loaders.Any(l => l.ContainsPlugin(pluginId));

        public bool ContainsTheme(string themeName)
            => m_Loaders.Any(l => l.ContainsTheme(themeName));

        public IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters)
        {
            var loader = m_Loaders.FirstOrDefault(l => l.ContainsComponent(componentName));

            if (loader != null)
            {
                return loader.LoadComponentFiles(componentName, filters);
            }
            else 
            {
                throw new LibraryItemLoadException(componentName, "", null);
            }
        }

        public IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters)
        {
            var loader = m_Loaders.FirstOrDefault(l => l.ContainsPlugin(pluginId));

            if (loader != null)
            {
                return loader.LoadPluginFiles(pluginId, filters);
            }
            else
            {
                throw new LibraryItemLoadException(pluginId, "", null);
            }
        }

        public IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters)
        {
            var loader = m_Loaders.FirstOrDefault(l => l.ContainsTheme(themeName));

            if (loader != null)
            {
                return loader.LoadThemeFiles(themeName, filters);
            }
            else
            {
                throw new LibraryItemLoadException(themeName, "", null);
            }
        }
    }
}
