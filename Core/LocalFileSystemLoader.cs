//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class LocalFileSystemLoaderConfig : ILoaderConfig
    {
        public string Path { get; }

        public LocalFileSystemLoaderConfig(string path) 
        {
            Path = path;
        }
    }

    public class LocalFileSystemLoader : ILoader
    {
        public ILoaderConfig Configuration { get; }

        public LocalFileSystemLoader(LocalFileSystemLoaderConfig config) 
        {
            Configuration = config;
        }

        public IEnumerable<ISourceFile> Load()
        {
            return null;
        }
    }
}
