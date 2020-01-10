//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Loader
{
    public class LocalFileSystemLoaderConfig
    {
        private const string IGNORE_FILE_PARAM_NAME = "ignore";

        public string Path { get; }
        public string[] Ignore { get; }

        public LocalFileSystemLoaderConfig(string path, string[] ignore)
        {
            Path = path;
            Ignore = ignore ?? new string[0];
        }

        public LocalFileSystemLoaderConfig(string path, Configuration conf) 
            : this(path, GetIgnoreFiles(conf))
        {
        }

        private static string[] GetIgnoreFiles(Configuration conf)
        {
            dynamic arr;
            
            if (conf.TryGetValue(IGNORE_FILE_PARAM_NAME, out arr))
            {
                if (arr is IEnumerable<string>)
                {
                    return (arr as IEnumerable<string>).ToArray();
                }
                else 
                {
                    throw new InvalidCastException($"Value specified in {IGNORE_FILE_PARAM_NAME} must be an array");
                }
            }
            else 
            {
                return null;
            }
        }
    }
}
