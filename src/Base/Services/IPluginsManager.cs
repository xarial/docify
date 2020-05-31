//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Services
{
    public interface IPluginsManager
    {
        Task LoadPlugins(IAsyncEnumerable<IFile> files);
    }
}
