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
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base;
using System.Composition.Convention;
using System.Threading.Tasks;

namespace Xarial.Docify.Core.Plugin
{
    public class PluginsManager : IPluginsManager
    {
        private const string PLUGIN_SETTINGS_TOKEN = "^";

        private IEnumerable<IPluginBase> m_Plugins;

        private readonly IDocifyApplication m_Engine;
        private readonly IConfiguration m_Conf;

        private bool m_IsLoaded;

        public PluginsManager(IConfiguration conf, IDocifyApplication engine)
        {
            m_Conf = conf;
            m_Engine = engine;

            m_IsLoaded = false;
        }

        public async Task LoadPlugins(IAsyncEnumerable<IFile> files)
        {
            if (!m_IsLoaded)
            {
                m_IsLoaded = true;

                var cb = new ConventionBuilder();

                cb.ForTypesMatching(t =>
                {
                    if (typeof(IPluginBase).IsAssignableFrom(t))
                    {
                        var id = GetPluginId(t);

                        return m_Conf.Plugins.Contains(id, StringComparer.InvariantCultureIgnoreCase);
                    }

                    return false;
                }).Export<IPluginBase>();

                var pluginAssemblies = new List<Assembly>();

                await foreach (var pluginFile in files) 
                {
                    var ext = Path.GetExtension(pluginFile.Location.FileName);

                    if(new string[] { ".dll", ".exe" }.Contains(ext, StringComparer.CurrentCultureIgnoreCase)) 
                    {
                        using (var assmStream = new MemoryStream(pluginFile.Content))
                        {
                            assmStream.Seek(0, SeekOrigin.Begin);
                            pluginAssemblies.Add(AssemblyLoadContext.Default.LoadFromStream(assmStream));
                        }
                    }
                }
                
                var configuration = new ContainerConfiguration()
                    .WithAssemblies(pluginAssemblies)
                    .WithDefaultConventions(cb);

                using (var host = configuration.CreateContainer())
                {
                    m_Plugins = host.GetExports<IPluginBase>();
                }

                if (m_Plugins == null)
                {
                    m_Plugins = Enumerable.Empty<IPluginBase>();
                }

                InitPlugins();
            }
            else 
            {
                throw new Exception("Plugins already loaded");
            }
        }
                
        private void InitPlugins()
        {
            foreach (var plugin in m_Plugins)
            {
                var pluginSpecType = plugin.GetType();

                if (plugin is IPlugin)
                {
                    (plugin as IPlugin).Init(m_Engine);
                }
                else if (IsAssignableToGenericType(pluginSpecType, typeof(IPlugin<>), out Type pluginDeclrType))
                {
                    var settsType = pluginDeclrType.GetGenericArguments().ElementAt(0);

                    var pluginId = GetPluginId(pluginSpecType);

                    IDictionary<string, object> settsData;

                    object setts = null;

                    if (MetadataExtension.TryGetParameter(m_Conf, PLUGIN_SETTINGS_TOKEN + pluginId, out settsData))
                    {
                        setts = MetadataExtension.ToObject(settsData, settsType);
                    }
                    else
                    {
                        setts = Activator.CreateInstance(settsType);
                    }

                    var initMethod = pluginSpecType.GetMethod(nameof(IPlugin<object>.Init));
                    initMethod.Invoke(plugin, new object[] { m_Engine, setts });
                }
                else
                {
                    throw new NotSupportedException($"'{plugin.GetType().FullName}' is not supported");
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
