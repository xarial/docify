using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface ILibraryLoader
    {
        IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string pattern = "*.*");
        IAsyncEnumerable<IFile> LoadComponentFiles(string componentName);
        IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId);
    }
}
