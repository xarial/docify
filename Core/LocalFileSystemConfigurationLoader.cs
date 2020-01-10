//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using YamlDotNet.Serialization;

namespace Xarial.Docify.Core
{
    public class LocalFileSystemConfigurationLoader : IConfigurationLoader
    {
        private const string CONF_FILE_NAME = "_config.yml";

        private readonly IFileSystem m_FileSystem;
        private readonly string m_ConfigFile;
        private readonly IDeserializer m_YamlSerializer;
        private readonly Environment_e m_Environment;

        public LocalFileSystemConfigurationLoader(string srcDir, Environment_e env)
            : this(srcDir, new FileSystem(), env)
        {
        }

        public LocalFileSystemConfigurationLoader(string srcDir, IFileSystem fileSystem, Environment_e env) 
        {
            m_ConfigFile = Path.Combine(srcDir, CONF_FILE_NAME);
            m_FileSystem = fileSystem;
            m_YamlSerializer = new DeserializerBuilder().Build();
            m_Environment = env;
        }

        public async Task<Configuration> Load()
        {
            if (m_FileSystem.File.Exists(m_ConfigFile))
            {
                var confStr = await m_FileSystem.File.ReadAllTextAsync(m_ConfigFile);

                return new Configuration(m_YamlSerializer.Deserialize<Dictionary<string, dynamic>>(confStr), m_Environment);
            }
            else 
            {
                return new Configuration(m_Environment);
            }
        }
    }
}
