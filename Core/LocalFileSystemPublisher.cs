//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Core
{
    public class LocalFileSystemPublisherConfig
    {
        public string OutDir { get; }

        public LocalFileSystemPublisherConfig(string outDir)
        {
            OutDir = outDir;
        }
    }

    public class LocalFileSystemPublisher : IPublisher
    {
        private readonly LocalFileSystemPublisherConfig m_Config;

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config) 
        {
            m_Config = config;
        }

        public void Write(string path, byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
