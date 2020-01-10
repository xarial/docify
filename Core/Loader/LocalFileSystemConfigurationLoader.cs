﻿//*********************************************************************
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

namespace Xarial.Docify.Core.Loader
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
        private const string DEFAULT_FRAGMENTS_DIR = "Fragments";
        private const string DEFAULT_THEMES_DIR = "Themes";

        private readonly IFileSystem m_FileSystem;
        private readonly IDeserializer m_YamlSerializer;
        private readonly Environment_e m_Environment;

        public LocalFileSystemConfigurationLoader(Environment_e env)
            : this(new FileSystem(), env)
        {
        }

        public LocalFileSystemConfigurationLoader(IFileSystem fileSystem, Environment_e env) 
        {
            m_FileSystem = fileSystem;
            m_YamlSerializer = new DeserializerBuilder().Build();
            m_Environment = env;
        }

        public async Task<Configuration> Load(Location location)
        {
            string NormalizeDirFunc(string dir, string defDir)
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
            };

            var conf = await GetConfiguration(location);

            var theme = conf.GetRemoveParameterOrDefault<string>(Params.Theme);
            var themesDir = NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.ThemesDir), DEFAULT_THEMES_DIR);

            if (!string.IsNullOrEmpty(theme))
            {
                var themeConf = await GetConfiguration(Location.FromPath(Path.Combine(themesDir, theme)));
                conf = conf.Merge(themeConf);
            }

            conf.Environment = m_Environment;
            conf.ThemesFolder = Location.FromPath(themesDir);
            conf.Theme = theme;

            conf.WorkingFolder = NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.WorkDir), Path.GetTempPath());
            conf.FragmentsFolder = Location.FromPath(NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.FragmentsDir), DEFAULT_FRAGMENTS_DIR));
            conf.Fragments = conf.GetRemoveParameterOrDefault<IEnumerable<object>>(Params.Fragments)?.Cast<string>()?.ToList();
            
            return conf;
        }

        private async Task<Configuration> GetConfiguration(Location location)
        {
            var srcDir = location.ToPath();

            var configFilePath = Path.Combine(srcDir, CONF_FILE_NAME);

            if (m_FileSystem.File.Exists(configFilePath))
            {
                var confStr = await m_FileSystem.File.ReadAllTextAsync(configFilePath);

                return new Configuration(m_YamlSerializer.Deserialize<Dictionary<string, dynamic>>(confStr));
            }
            else
            {
                return new Configuration();
            }
        }
    }
}
