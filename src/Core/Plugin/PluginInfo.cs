//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Plugin
{
    public class PluginInfo : IPluginInfo
    {
        public string Name { get; }
        public IAsyncEnumerable<IFile> Files { get; }

        public PluginInfo(string name, IAsyncEnumerable<IFile> files) 
        {
            Name = name;
            Files = files;
        }
    }
}
