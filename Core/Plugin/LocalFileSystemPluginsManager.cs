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

namespace Xarial.Docify.Core.Plugin
{
    public class LocalFileSystemPluginsManager : IPluginsManager
    {
        private readonly IFileSystem m_FileSystem;

        private readonly IEnumerable<IPlugin> m_Plugins;

        public LocalFileSystemPluginsManager(Configuration conf)
            : this(conf, new FileSystem())
        {
        }

        public LocalFileSystemPluginsManager(Configuration conf, IFileSystem fileSystem)
        {
            m_FileSystem = fileSystem;
            
            if (conf.Plugins?.Any() == true)
            {
                var cb = new ConventionBuilder();

                cb.ForTypesMatching(t =>
                {
                    if (typeof(IPlugin).IsAssignableFrom(t))
                    {
                        var id = GetPluginId(t);

                        return conf.Plugins.Contains(id, StringComparer.InvariantCultureIgnoreCase);
                    }

                    return false;
                }).Export<IPlugin>();
                                
                var pluginAssemblies = m_FileSystem.Directory.GetFiles(conf.PluginsFolder.ToPath(), "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(f => AssemblyLoadContext.Default.LoadFromStream(m_FileSystem.File.OpenRead(f)))
                    .Where(s => s.GetTypes().Where(p => typeof(IPlugin).IsAssignableFrom(p)).Any());

                var configuration = new ContainerConfiguration()
                    .WithAssemblies(pluginAssemblies)
                    .WithDefaultConventions(cb);

                using (var host = configuration.CreateContainer()) 
                {
                    m_Plugins = host.GetExports<IPlugin>();
                }

                LoadPluginSettings(conf);
            }
        }

        private void LoadPluginSettings(Configuration conf)
        {
            if (m_Plugins != null)
            {
                foreach (var plugin in m_Plugins)
                {
                    var pluginSpecType = plugin.GetType();

                    if (IsAssignableToGenericType(pluginSpecType, typeof(IPlugin<>)))
                    {
                        var prp = pluginSpecType.GetProperty(nameof(IPlugin<object>.Settings));
                        var settsType = prp.PropertyType;

                        var pluginId = GetPluginId(pluginSpecType);

                        dynamic settsData;

                        object setts = null;

                        if (conf.TryGetValue(pluginId, out settsData))
                        {
                            setts = MetadataExtension.ToObject(settsData, settsType);
                        }
                        else
                        {
                            setts = Activator.CreateInstance(settsType);
                        }

                        prp.SetValue(plugin, setts);
                    }
                }
            }
        }

        public void LoadPlugins<T>(T service)
        {
            if (m_Plugins != null)
            {
                foreach (var importField in service.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Where(f => typeof(IEnumerable<IPlugin>).IsAssignableFrom(f.FieldType)))
                {
                    var importPlugins = m_Plugins.Where(p => importField.FieldType.IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(p.GetType())));

                    if (importPlugins.Any())
                    {
                        var pluginType = importField.FieldType.GetGenericArguments().First();
                        var pluginsListType = typeof(List<>).MakeGenericType(pluginType);
                        var pluginsList = Activator.CreateInstance(pluginsListType) as IList;

                        foreach (var importPlugin in importPlugins) 
                        {
                            pluginsList.Add(importPlugin);
                        }

                        importField.SetValue(service, pluginsList);
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

        private bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            foreach (var it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            var baseType = givenType.BaseType;

            if (baseType == null)
            {
                return false; 
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
