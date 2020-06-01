using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Loader
{
    public class LibraryLoader : ILibraryLoader
    {
        //TODO: implement support for multiple libraries

        private readonly ILibraryLoader[] m_Loaders;

        public LibraryLoader(ILibraryLoader[] loaders) 
        {
            m_Loaders = loaders;
        }

        public IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters)
        {
            return m_Loaders.First().LoadComponentFiles(componentName, filters);
        }

        public IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters)
        {
            return m_Loaders.First().LoadPluginFiles(pluginId, filters);
        }

        public IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters)
        {
            return m_Loaders.First().LoadThemeFiles(themeName, filters);
        }
    }
}
