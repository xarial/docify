//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin
{
    public static class IPluginExtension
    {
        public static void InvokePluginsIfAny<T>(this IEnumerable<T> plugins, Action<T> invoker)
            where T : IPlugin
        {
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    invoker.Invoke(plugin);
                }
            }
        }

        public static async Task InvokePluginsIfAnyAsync<T>(this IEnumerable<T> plugins, Func<T, Task> invoker)
            where T : IPlugin
        {
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    await invoker.Invoke(plugin);
                }
            }
        }
    }
}
