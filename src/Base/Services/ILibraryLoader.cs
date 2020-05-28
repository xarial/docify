using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface ILibraryLoader
    {
        IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters = null);
        IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters = null);
        IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters = null);
    }
}
