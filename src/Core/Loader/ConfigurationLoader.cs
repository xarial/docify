//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using System.Linq;
using Xarial.Docify.Core.Helpers;

namespace Xarial.Docify.Core.Loader
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        private class Params
        {
            internal const string Components = "components";
            internal const string Theme = "theme";
            internal const string Plugins = "plugins";
        }

        private const string CONF_FILE_NAME = "_config.yml";

        private readonly MetadataSerializer m_ConfigSerializer;
        private readonly string m_Environment;
        private readonly IFileLoader m_FileLoader;
        private readonly ILibraryLoader m_LibraryLoader;

        private readonly string m_EnvConfFileName;

        public ConfigurationLoader(IFileLoader fileLoader, ILibraryLoader libraryLoader, string env)
        {
            m_ConfigSerializer = new MetadataSerializer();
            m_FileLoader = fileLoader;
            m_Environment = env;
            m_LibraryLoader = libraryLoader;

            m_EnvConfFileName = Path.GetFileNameWithoutExtension(CONF_FILE_NAME) + "." + m_Environment.ToString()
                    + Path.GetExtension(CONF_FILE_NAME);
        }

        public async Task<IConfiguration> Load(ILocation[] locations)
        {
            IConfiguration conf = new Configuration();

            foreach (var loc in locations)
            {
                var confs = await LoadConfigurationsFromLocation(loc);

                conf = confs.Conf.Merge(conf);
                conf = confs.EnvConf.Merge(conf);
            }

            var themesHierarchy = new List<string>();
            string theme;

            do
            {
                theme = conf.GetRemoveParameterOrDefault<string>(Params.Theme);

                if (!string.IsNullOrEmpty(theme))
                {
                    IConfiguration themeConf = new Configuration();
                    IConfiguration themeEnvConf = new Configuration();

                    await foreach (var themeConfFile in m_LibraryLoader.LoadThemeFiles(theme,
                        new string[] { CONF_FILE_NAME, m_EnvConfFileName }))
                    {
                        if (string.Equals(themeConfFile.Location.FileName, CONF_FILE_NAME, StringComparison.CurrentCultureIgnoreCase))
                        {
                            themeConf = ConfigurationFromFile(themeConfFile);
                        }
                        else if (string.Equals(themeConfFile.Location.FileName, m_EnvConfFileName, StringComparison.CurrentCultureIgnoreCase))
                        {
                            themeEnvConf = ConfigurationFromFile(themeConfFile);
                        }
                        else
                        {
                            throw new Exception("Invalid theme configuration file");
                        }
                    }

                    conf = conf.Merge(themeEnvConf);
                    conf = conf.Merge(themeConf);

                    themesHierarchy.Add(theme);
                }
            }
            while (!string.IsNullOrEmpty(theme));

            conf.Environment = m_Environment;
            conf.ThemesHierarchy.AddRange(themesHierarchy);
            conf.Components = conf.GetRemoveParameterOrDefault<IEnumerable<string>>(Params.Components)?.ToList();
            conf.Plugins = conf.GetRemoveParameterOrDefault<IEnumerable<string>>(Params.Plugins)?.ToList();

            return conf;
        }

        private async Task<(IConfiguration Conf, IConfiguration EnvConf)> LoadConfigurationsFromLocation(ILocation loc)
        {
            var res = (Conf: (IConfiguration)new Configuration(), EnvConf: (IConfiguration)new Configuration());

            await foreach (var file in m_FileLoader.LoadFolder(loc, new string[]
            {
                CONF_FILE_NAME, m_EnvConfFileName
            }))
            {
                if (string.Equals(file.Location.FileName, CONF_FILE_NAME, StringComparison.CurrentCultureIgnoreCase))
                {
                    res.Conf = ConfigurationFromFile(file);
                }
                else if (string.Equals(file.Location.FileName, m_EnvConfFileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    res.EnvConf = ConfigurationFromFile(file);
                }
                else
                {
                    throw new Exception("Invalid configuration file");
                }
            }

            return res;
        }

        private IConfiguration ConfigurationFromFile(IFile file)
        {
            var confStr = file.AsTextContent();
            return new Configuration(m_ConfigSerializer.Deserialize<Dictionary<string, object>>(confStr));
        }
    }
}
