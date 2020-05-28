//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using YamlDotNet.Serialization;
using Xarial.Docify.Core.Data;
using System.Linq;
using Xarial.Docify.Core.Helpers;

namespace Xarial.Docify.Core.Loader
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        private class Params
        {
            //internal const string WorkDir = "work_dir";
            //internal const string ComponentsDir = "components_dir";
            internal const string Components = "components";
            //internal const string ThemesDir = "themes_dir";
            internal const string Theme = "theme";
            //internal const string PluginsDir = "plugins_dir";
            internal const string Plugins = "plugins";
        }

        private const string CONF_FILE_NAME = "_config.yml";
        //private const string DEFAULT_COMPONENTS_DIR = "Components";
        //private const string DEFAULT_THEMES_DIR = "Themes";
        //private const string DEFAULT_PLUGINS_DIR = "Plugins";

        //private readonly IFileSystem m_FileSystem;
        private readonly MetadataSerializer m_ConfigSerializer;
        private readonly string m_Environment;
        private readonly IFileLoader m_FileLoader;
        private readonly ILibraryLoader m_LibraryLoader;

        private readonly string m_EnvConfFileName;

        public ConfigurationLoader(IFileLoader fileLoader, ILibraryLoader libraryLoader, string env) 
        {
            //m_FileSystem = fileSystem;
            m_ConfigSerializer = new MetadataSerializer();
            m_FileLoader = fileLoader;
            m_Environment = env;
            m_LibraryLoader = libraryLoader;

            m_EnvConfFileName = Path.GetFileNameWithoutExtension(CONF_FILE_NAME) + "." + m_Environment.ToString()
                    + Path.GetExtension(CONF_FILE_NAME);
        }

        public async Task<IConfiguration> Load(ILocation[] locations)
        {
            //string NormalizeDirFunc(string dir, string defDir)
            //{
            //    var newDir = dir;

            //    if (string.IsNullOrEmpty(newDir))
            //    {
            //        newDir = defDir;
            //    }

            //    if (!Path.IsPathRooted(newDir))
            //    {
            //        var appDir = Path.GetDirectoryName(this.GetType().Assembly.Location);

            //        newDir = Path.Combine(appDir, newDir);
            //    }

            //    return newDir;
            //};

            IConfiguration conf = new Configuration();

            foreach (var loc in locations)
            {
                var confLoc = loc.Copy(CONF_FILE_NAME, loc.Path);
                var thisConf = await LoadConfigurationFromLocationIfExists(confLoc);
                
                if(thisConf != null) 
                {
                    conf = conf.Merge(thisConf);
                }

                var envConfFileLoc = confLoc.Copy(m_EnvConfFileName, confLoc.Path);
                thisConf = await LoadConfigurationFromLocationIfExists(envConfFileLoc);

                conf = conf.Merge(thisConf);
            }

            //var themesDir = NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.ThemesDir), DEFAULT_THEMES_DIR);

            var themesHierarchy = new List<string>();
            string theme;
            
            do
            {
                theme = conf.GetRemoveParameterOrDefault<string>(Params.Theme);

                if (!string.IsNullOrEmpty(theme))
                {
                    await foreach (var themeConfFile in m_LibraryLoader.LoadThemeFiles(theme, "*::" + CONF_FILE_NAME)) 
                    {
                        var themeConf = ConfigurationFromFile(themeConfFile);
                        conf = conf.Merge(themeConf);
                    }

                    await foreach (var themeConfFile in m_LibraryLoader.LoadThemeFiles(theme, "*::" + m_EnvConfFileName))
                    {
                        var themeConf = ConfigurationFromFile(themeConfFile);
                        conf = conf.Merge(themeConf);
                    }

                    themesHierarchy.Add(theme);
                }
            }
            while (!string.IsNullOrEmpty(theme));
            
            conf.Environment = m_Environment;
            //conf.ThemesFolder = Location.FromPath(themesDir);
            conf.ThemesHierarchy.AddRange(themesHierarchy);
            
            //conf.WorkingFolder = NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.WorkDir), Path.GetTempPath());
            //conf.ComponentsFolder = Location.FromPath(NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.ComponentsDir), DEFAULT_COMPONENTS_DIR));
            conf.Components = conf.GetRemoveParameterOrDefault<IEnumerable<object>>(Params.Components)?.Cast<string>()?.ToList();
            //conf.PluginsFolder = Location.FromPath(NormalizeDirFunc(conf.GetRemoveParameterOrDefault<string>(Params.PluginsDir), DEFAULT_PLUGINS_DIR));
            conf.Plugins = conf.GetRemoveParameterOrDefault<IEnumerable<object>>(Params.Plugins)?.Cast<string>()?.ToList();

            return conf;
        }

        private async Task<IConfiguration> LoadConfigurationFromLocationIfExists(ILocation loc)
        {
            try
            {
                var confFile = await m_FileLoader.LoadFile(loc);
                return ConfigurationFromFile(confFile);
            }
            catch (FileNotFoundException)
            {
            }

            return null;
        }

        private IConfiguration ConfigurationFromFile(IFile file) 
        {
            var confStr = file.AsTextContent();
            return new Configuration(m_ConfigSerializer.Deserialize<Dictionary<string, object>>(confStr));
        }

        //private async Task<Configuration> GetConfiguration(ILocation confFileLocation)
        //{
        //    var conf = new Configuration();

            

        //    await LoadConfigurationFromLocationIfExists(confFileLocation);

        //    var envConfFileLoc = confFileLocation.Copy(m_EnvConfFileName, confFileLocation.Path);
        //    await LoadConfigurationFromLocationIfExists(envConfFileLoc);

        //    return conf;
        //}
    }
}
