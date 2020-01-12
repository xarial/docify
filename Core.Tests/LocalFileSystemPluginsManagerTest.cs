//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core.Plugin;

namespace Core.Tests
{
    public class LocalFileSystemPluginsManagerTest
    {
        public interface IPlugin1 : IPlugin 
        {
        }

        public interface IPlugin2 : IPlugin
        {
        }

        public class Plugin1 : IPlugin1 
        {
        }

        public class Plugin2 : IPlugin2
        {
        }

        public class Plugin3 : IPlugin1, IPlugin2
        {
        }

        public class ServiceMock1 
        {
#pragma warning disable CS0169 // Add readonly modifier
            private IEnumerable<IPlugin1> m_Plugins1;
#pragma warning restore CS0169 // Add readonly modifier
            protected IEnumerable<IPlugin2> m_Plugins2;
        }

        [Test]
        public void InitTest() 
        {
            var assmBuilder = System.Reflection.Emit.AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Plugin.Test"), 
                System.Reflection.Emit.AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder = assmBuilder.DefineDynamicModule(Guid.NewGuid().ToString());
            
            var typeBuilder1 = moduleBuilder.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder1.AddInterfaceImplementation(typeof(IPlugin));
            typeBuilder1.CreateType();

            var typeBuilder2 = moduleBuilder.DefineType("Plugin2", TypeAttributes.Public);
            typeBuilder2.AddInterfaceImplementation(typeof(IPlugin));
            var p2 = typeBuilder2.CreateType();

            var assm = moduleBuilder.Assembly;

            var assmBuffer = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm);

            var fs = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
            fs.AddFile("D:\\Plugins\\mockplugins.dll", new System.IO.Abstractions.TestingHelpers.MockFileData(assmBuffer));
            
            var mgr = new LocalFileSystemPluginsManager(new Configuration()
            {
                PluginsFolder = Location.FromPath("D:\\Plugins"),
                Plugins = new string[] { "plugin2" }.ToList()
            }, fs);

            var res = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mgr) as IEnumerable<IPlugin>;

            Assert.AreEqual(1, res.Count());
            Assert.IsInstanceOf(typeof(IPlugin), res.First());
            Assert.AreEqual(p2.FullName, res.First().GetType().FullName);
        }

        [Test]
        public void LoadPluginsTest() 
        {
            var mgr = new LocalFileSystemPluginsManager(new Configuration());
            var field = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic);
            var p1 = new Plugin1();
            var p2 = new Plugin2();
            var p3 = new Plugin3();
            field.SetValue(mgr, new IPlugin[] { p1, p2, p3 });
            
            var svcMock = new ServiceMock1();
            mgr.LoadPlugins(svcMock);

            var res1 = svcMock.GetType().GetField("m_Plugins1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(svcMock) as IEnumerable<IPlugin1>;
            var res2 = svcMock.GetType().GetField("m_Plugins2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(svcMock) as IEnumerable<IPlugin2>;

            Assert.AreEqual(2, res1.Count());
            Assert.AreEqual(2, res2.Count());
            Assert.That(res1.Contains(p1));
            Assert.That(res1.Contains(p3));
            Assert.That(res2.Contains(p2));
            Assert.That(res2.Contains(p3));
        }
    }
}
