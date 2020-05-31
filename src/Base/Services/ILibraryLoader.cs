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
    public interface ILibraryLoader
    {
        IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters);
        IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters);
        IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters);
    }
}
