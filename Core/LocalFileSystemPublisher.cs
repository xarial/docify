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
    public class LocalFileSystemPublisherConfig : IPublisherConfig
    {
        public string OutDir { get; }

        public LocalFileSystemPublisherConfig(string outDir)
        {
            OutDir = outDir;
        }
    }

    public class LocalFileSystemPublisher : IPublisher
    {
        public IPublisherConfig Config { get; }

        public LocalFileSystemPublisher(LocalFileSystemPublisherConfig config) 
        {
            Config = config;
        }

        public void Write(string path, byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
