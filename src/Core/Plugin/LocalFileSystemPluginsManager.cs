//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base;
using System.Composition.Convention;
using System.Composition;
using System.Collections;
using System.Composition.Hosting.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.Text.RegularExpressions;
using Xarial.Docify.Core.Data;
using System.Threading.Tasks;

namespace Xarial.Docify.Core.Plugin
{
    public class LocalFileSystemPluginsManager : IPluginsManager
    {
        private const string PLUGIN_SETTINGS_TOKEN = "^";

        private IEnumerable<IPluginBase> m_Plugins;

        private readonly IDocifyApplication m_Engine;
        private readonly IConfiguration m_Conf;
        private readonly IFileSystem m_FileSystem;

        private bool m_IsLoaded;

        public LocalFileSystemPluginsManager(IConfiguration conf, IDocifyApplication engine)
            : this(conf, new FileSystem(), engine)
        {
        }

        public LocalFileSystemPluginsManager(IConfiguration conf, IFileSystem fileSystem, IDocifyApplication engine)
        {
            m_Conf = conf;
            m_FileSystem = fileSystem;
            m_Engine = engine;

            m_IsLoaded = false;
        }

        public Task LoadPlugins()
        {
            if (!m_IsLoaded)
            {
                m_IsLoaded = true;
                m_Plugins = LoadPlugins(m_Conf, m_FileSystem, m_Engine);
                return Task.CompletedTask;
            }
            else 
            {
                throw new Exception("Plugins already loaded");
            }
        }

        private IEnumerable<IPluginBase> LoadPlugins(IConfiguration conf, IFileSystem fileSystem, IDocifyApplication engine) 
        {
            IEnumerable<IPluginBase> plugins = null;

            if (conf.Plugins?.Any() == true)
            {
                var cb = new ConventionBuilder();

                cb.ForTypesMatching(t =>
                {
                    if (typeof(IPluginBase).IsAssignableFrom(t))
                    {
                        var id = GetPluginId(t);

                        return conf.Plugins.Contains(id, StringComparer.InvariantCultureIgnoreCase);
                    }

                    return false;
                }).Export<IPluginBase>();

                var pluginAssemblies = fileSystem.Directory.GetFiles(conf.PluginsFolder.ToPath(), "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(f => AssemblyLoadContext.Default.LoadFromStream(fileSystem.File.OpenRead(f)))
                    .Where(s => s.GetTypes().Where(p => typeof(IPluginBase).IsAssignableFrom(p)).Any());

                var configuration = new ContainerConfiguration()
                    .WithAssemblies(pluginAssemblies)
                    .WithDefaultConventions(cb);

                using (var host = configuration.CreateContainer())
                {
                    plugins = host.GetExports<IPluginBase>();
                }

                if (plugins == null)
                {
                    plugins = Enumerable.Empty<IPluginBase>();
                }

                InitPlugins(plugins, conf, engine);
            }

            return plugins;
        }
        
        private void InitPlugins(IEnumerable<IPluginBase> plugins, IConfiguration conf, IDocifyApplication engine)
        {
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    var pluginSpecType = plugin.GetType();

                    if (plugin is IPlugin)
                    {
                        (plugin as IPlugin).Init(engine);
                    }
                    else if (IsAssignableToGenericType(pluginSpecType, typeof(IPlugin<>), out Type pluginDeclrType))
                    {
                        var settsType = pluginDeclrType.GetGenericArguments().ElementAt(0);

                        var pluginId = GetPluginId(pluginSpecType);

                        IDictionary<string, object> settsData;

                        object setts = null;

                        if (MetadataExtension.TryGetParameter(conf, PLUGIN_SETTINGS_TOKEN + pluginId, out settsData))
                        {
                            setts = MetadataExtension.ToObject(settsData, settsType);
                        }
                        else
                        {
                            setts = Activator.CreateInstance(settsType);
                        }

                        var initMethod = pluginSpecType.GetMethod(nameof(IPlugin<object>.Init));
                        initMethod.Invoke(plugin, new object[] { engine, setts });
                    }
                    else 
                    {
                        throw new NotSupportedException($"'{plugin.GetType().FullName}' is not supported");
                    }
                }
            }
        }

        private string GetPluginId(Type pluginType)
        {
            var id = pluginType.GetCustomAttribute<PluginAttribute>()?.Id;

            if (string.IsNullOrEmpty(id))
            {
                id = pluginType.FullName;
            }

            return id;
        }

        private bool IsAssignableToGenericType(Type givenType, Type genericType, out Type specGenericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    specGenericType = it;
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                specGenericType = givenType;
                return true;
            }

            var baseType = givenType.BaseType;

            if (baseType == null)
            {
                specGenericType = null;
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType, out specGenericType);
        }
    }
}
