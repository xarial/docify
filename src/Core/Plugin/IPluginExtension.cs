////*********************************************************************
////docify
////Copyright(C) 2020 Xarial Pty Limited
////Product URL: https://www.docify.net
////License: https://github.com/xarial/docify/blob/master/LICENSE
////*********************************************************************

//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using Xarial.Docify.Base.Plugins;

//namespace Xarial.Docify.Core.Plugin
//{
//    public static class IPluginExtension
//    {
//        public static void InvokePluginsIfAny<TPlugin>(this IEnumerable<TPlugin> plugins, Action<TPlugin> invoker)
//            where TPlugin : IPlugin
//        {
//            if (plugins != null)
//            {
//                foreach (var plugin in plugins)
//                {
//                    invoker.Invoke(plugin);
//                }
//            }
//        }

//        public static async Task InvokePluginsIfAnyAsync<TPlugin>(this IEnumerable<TPlugin> plugins, Func<TPlugin, Task> invoker)
//            where TPlugin : IPlugin
//        {
//            if (plugins != null)
//            {
//                foreach (var plugin in plugins)
//                {
//                    await invoker.Invoke(plugin);
//                }
//            }
//        }

//        public static async IAsyncEnumerable<TResult> InvokePluginsIfAnyAsync<TPlugin, TResult>(
//            this IEnumerable<TPlugin> plugins,
//            Func<TPlugin, IAsyncEnumerable<TResult>> invoker)
//            where TPlugin : IPlugin
//        {
//            if (plugins != null)
//            {
//                foreach (var plugin in plugins)
//                {
//                    await foreach (var res in invoker.Invoke(plugin)) 
//                    {
//                        yield return res;
//                    }
//                }
//            }
//        }
//    }
//}
