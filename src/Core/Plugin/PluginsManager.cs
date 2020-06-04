//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using System.Composition.Convention;
using System.Threading.Tasks;
using Xarial.XToolkit.Reflection;
using System.Diagnostics.CodeAnalysis;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Plugin
{
    public class PluginsManager : IPluginsManager
    {
        private class AssemblyComparer : IEqualityComparer<Assembly>
        {
            public bool Equals([AllowNull] Assembly x, [AllowNull] Assembly y)
            {
                if (object.ReferenceEquals(x, y)) 
                {
                    return true;
                }

                if (x == null || y == null) 
                {
                    return false;
                }

                return x.GetName() == y.GetName();
            }

            public int GetHashCode([DisallowNull] Assembly obj)
            {
                return 0;
            }
        }

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

        public async Task LoadPlugins(IAsyncEnumerable<IPluginInfo> pluginInfos)
        {
            if (!m_IsLoaded)
            {
                m_IsLoaded = true;

                var cb = new ConventionBuilder();

                var pluginAssemblies = new Dictionary<Assembly, string>(new AssemblyComparer());
                var loadedPlugins = new List<string>();

                cb.ForTypesMatching(t =>
                {
                    if (typeof(IPluginBase).IsAssignableFrom(t))
                    {
                        if (pluginAssemblies.TryGetValue(t.Assembly, out string id))
                        {
                            if (!loadedPlugins.Contains(id, StringComparer.CurrentCultureIgnoreCase))
                            {
                                loadedPlugins.Add(id);
                                return true;
                            }
                            else
                            {
                                throw new MultiplePluginsPerNameException(id);
                            }
                        }
                    }

                    return false;
                }).Export<IPluginBase>();

                var discoveredPlugins = new List<string>();

                await foreach (var pluginInfo in pluginInfos)
                {
                    if (!discoveredPlugins.Contains(pluginInfo.Name, StringComparer.CurrentCultureIgnoreCase))
                    {
                        discoveredPlugins.Add(pluginInfo.Name);
                    }
                    else 
                    {
                        throw new DuplicatePluginException(pluginInfo.Name);
                    }

                    await foreach (var pluginFile in pluginInfo.Files)
                    {
                        var ext = Path.GetExtension(pluginFile.Location.FileName);

                        if (string.Equals(ext, ".dll", StringComparison.CurrentCultureIgnoreCase))
                        {
                            using (var assmStream = new MemoryStream(pluginFile.Content))
                            {
                                assmStream.Seek(0, SeekOrigin.Begin);
                                var assm = AssemblyLoadContext.Default.LoadFromStream(assmStream);

                                if (!pluginAssemblies.ContainsKey(assm))
                                {
                                    pluginAssemblies.Add(assm, pluginInfo.Name);
                                }
                            }
                        }
                    }
                }

                var configuration = new ContainerConfiguration()
                    .WithAssemblies(pluginAssemblies.Keys)
                    .WithDefaultConventions(cb);

                using (var host = configuration.CreateContainer())
                {
                    var plugins = host.GetExports<IPluginBase>();
                    m_Plugins = plugins.OrderBy(p => m_Conf.Plugins.FindIndex(
                        x => string.Equals(x, pluginAssemblies[p.GetType().Assembly],
                        StringComparison.CurrentCultureIgnoreCase)));
                }

                var notLoadedPlugins = pluginAssemblies.Values.Distinct(
                    StringComparer.CurrentCultureIgnoreCase).Except(loadedPlugins);

                if (notLoadedPlugins.Any()) 
                {
                    throw new MissingPluginImplementationException(notLoadedPlugins);
                }

                if (m_Plugins == null)
                {
                    m_Plugins = Enumerable.Empty<IPluginBase>();
                }

                InitPlugins(pluginAssemblies);
            }
            else
            {
                throw new Exception("Plugins already loaded");
            }
        }

        private void InitPlugins(Dictionary<Assembly, string> pluginAssemblies)
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

                    var pluginName = pluginAssemblies[pluginSpecType.Assembly];

                    IDictionary<string, object> settsData;

                    object setts = null;

                    if (MetadataExtension.TryGetParameter(m_Conf, PLUGIN_SETTINGS_TOKEN + pluginName, out settsData))
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

        private bool IsAssignableToGenericType(Type givenType, Type genericType, out Type specGenericType)
        {
            specGenericType = givenType.TryFindGenericType(genericType);
            return specGenericType != null;
        }
    }
}
