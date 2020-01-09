﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemLoaderConfig
    {
        public string Path { get; }
        public string[] Ignore { get; }

        public LocalFileSystemLoaderConfig(string path, string[] ignore)
        {
            Path = path;
            Ignore = ignore ?? new string[0];
        }
    }
}
