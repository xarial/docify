//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemLoader : ILoader
    {
        private readonly LocalFileSystemLoaderConfig m_Config;

        public LocalFileSystemLoader(LocalFileSystemLoaderConfig config) 
        {
            m_Config = config;
        }

        public IEnumerable<ISourceFile> Load()
        {
            return null;
        }
    }
}
