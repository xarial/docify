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
using Xarial.Docify.Core.Data;
using System.Linq;

namespace Xarial.Docify.Core
{
    public class LocalFileSystemConfigurationLoader : IConfigurationLoader
    {
        private class Params
        {
            internal const string WorkDir = "work_dir";
            internal const string FragmentsDir = "fragments_dir";
            internal const string Fragments = "fragments";
            internal const string ThemesDir = "themes_dir";
            internal const string Theme = "theme";
        }

        private const string CONF_FILE_NAME = "_config.yml";
        private const string DEFAULT_FRAGMENTS_DIR = "fragments";
        private const string DEFAULT_THEMES_DIR = "themes";

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
            Configuration conf;

            if (m_FileSystem.File.Exists(m_ConfigFile))
            {
                var confStr = await m_FileSystem.File.ReadAllTextAsync(m_ConfigFile);

                conf = new Configuration(m_YamlSerializer.Deserialize<Dictionary<string, dynamic>>(confStr), m_Environment);

                conf.WorkingFolder = conf.GetRemoveParameterOrDefault<string>(Params.WorkDir);
                conf.FragmentsFolder = conf.GetRemoveParameterOrDefault<string>(Params.FragmentsDir);
                conf.ThemesFolder = conf.GetRemoveParameterOrDefault<string>(Params.ThemesDir);
                conf.Fragments = conf.GetRemoveParameterOrDefault<IEnumerable<object>>(Params.Fragments)?.Cast<string>()?.ToList();
                conf.Theme = conf.GetRemoveParameterOrDefault<string>(Params.Theme);

                return conf;
            }
            else 
            {
                conf = new Configuration(m_Environment);
            }

            var normalizeDir = new Func<string, string, string>((dir, defDir) => 
            {
                var newDir = dir;

                if (string.IsNullOrEmpty(newDir))
                {
                    newDir = defDir;
                }

                if (!Path.IsPathRooted(newDir))
                {
                    var appDir = Path.GetDirectoryName(this.GetType().Assembly.Location);

                    newDir = Path.Combine(appDir, newDir);
                }

                return newDir;
            });


            conf.WorkingFolder = normalizeDir.Invoke(conf.WorkingFolder, Path.GetTempPath());
            conf.FragmentsFolder = normalizeDir.Invoke(conf.FragmentsFolder, DEFAULT_FRAGMENTS_DIR);
            conf.ThemesFolder = normalizeDir.Invoke(conf.ThemesFolder, DEFAULT_THEMES_DIR);

            return conf;
        }
    }
}
