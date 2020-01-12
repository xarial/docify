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

namespace Xarial.Docify.Core.Plugin
{
    public class LocalFileSystemPluginsManager : IPluginsManager
    {
        private class PluginSelectorAttributedModelProvider : AttributedModelProvider
        {
            private readonly IEnumerable<string> m_Plugins;

            internal PluginSelectorAttributedModelProvider(IEnumerable<string> plugins) 
            {
                m_Plugins = plugins;
            }

            public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, MemberInfo member)
            {
                return ProcessType(reflectedType);
            }

            public override IEnumerable<Attribute> GetCustomAttributes(Type reflectedType, ParameterInfo parameter)
            {
                return ProcessType(reflectedType);
            }

            private IEnumerable<Attribute> ProcessType(Type reflectedType) 
            {
                if (typeof(IPlugin).IsAssignableFrom(reflectedType))
                {
                    var id = reflectedType.GetCustomAttribute<PluginAttribute>()?.Id;

                    if (string.IsNullOrEmpty(id)) 
                    {
                        id = reflectedType.FullName;
                    }

                    if (m_Plugins.Contains(id, StringComparer.InvariantCultureIgnoreCase))
                    {
                        yield return new ExportAttribute(typeof(IPlugin));
                    }
                }
            }
        }

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
                var pluginAssemblies = m_FileSystem.Directory.GetFiles(conf.PluginsFolder.ToPath(), "*.dll", SearchOption.TopDirectoryOnly)
                    .Select(f => AssemblyLoadContext.Default.LoadFromStream(m_FileSystem.File.OpenRead(f)))
                    .Where(s => s.GetTypes().Where(p => typeof(IPlugin).IsAssignableFrom(p)).Any());
                
                var configuration = new ContainerConfiguration()
                    .WithAssemblies(pluginAssemblies)
                    .WithDefaultConventions(new PluginSelectorAttributedModelProvider(conf.Plugins));

                using (var host = configuration.CreateContainer()) 
                {
                    m_Plugins = host.GetExports<IPlugin>();
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
    }
}
