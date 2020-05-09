//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Helpers;
using Xarial.Docify.Core.Plugin;
using YamlDotNet.Serialization;

namespace Core.Tests
{
    public class PluginMock<TSetts> : IPlugin<TSetts>
        where TSetts : new()
    {
        public TSetts Settings { get; private set; }

        public void Init(TSetts setts)
        {
            Settings = setts;
        }
    }

    public class MockSettings1
    {
        public string Prp1 { get; set; } = "A";
        public double PropTwo { get; set; }
        public string[] Prp3 { get; set; }
    }
    
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

        public class Plugin4 : IPlugin
        {
            [Import]
            private IServiceMock2 Service { get; set; }
        }

        public class ServiceMock1 
        {
            [ImportPlugins]
            private IEnumerable<IPlugin1> m_Plugins1;

            [ImportPlugins]
            protected IEnumerable<IPlugin2> m_Plugins2;
        }

        public interface IServiceMock2
        {
        }

        public class ServiceMock2 : IServiceMock2
        {
        }

        [Test]
        public void InitTest() 
        {
            var assmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), 
                AssemblyBuilderAccess.RunAndCollect);
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
        public void LoadDefaultSettingsTest()
        {
            var assmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);

            var moduleBuilder = assmBuilder.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder = moduleBuilder.DefineType("Plugin1", TypeAttributes.Public);

            //need this line to properly load Base.dll
            typeBuilder.AddInterfaceImplementation(typeof(IPlugin<MockSettings1>));
            typeBuilder.SetParent(typeof(PluginMock<MockSettings1>));

            typeBuilder.CreateType();

            var assm = moduleBuilder.Assembly;

            var assmBuffer = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm);

            var fs = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
            fs.AddFile("D:\\Plugins\\mockplugins.dll", new System.IO.Abstractions.TestingHelpers.MockFileData(assmBuffer));

            var conf = new Configuration();
            conf.PluginsFolder = Location.FromPath("D:\\Plugins");
            conf.Plugins = new List<string>(new string[] { "plugin1" });

            var mgr = new LocalFileSystemPluginsManager(conf, fs);

            var res = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mgr) as IEnumerable<IPlugin>;
            var plg = res.OfType<PluginMock<MockSettings1>>().FirstOrDefault();

            Assert.IsNotNull(plg);
            Assert.IsNotNull(plg.Settings);
            Assert.AreEqual("A", plg.Settings.Prp1);
            Assert.AreEqual(0, plg.Settings.PropTwo);
            Assert.IsNull(plg.Settings.Prp3);
        }
        
        [Test]
        public void LoadSettingsTest()
        {
            var assmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);

            var moduleBuilder = assmBuilder.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder = moduleBuilder.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof(PluginAttribute).GetConstructor(
                new Type[] { typeof(string) }), new object[] { "plg1" }));

            typeBuilder.SetParent(typeof(PluginMock<MockSettings1>));
            
            typeBuilder.CreateType();

            var assm = moduleBuilder.Assembly;

            var assmBuffer = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm);

            var fs = new System.IO.Abstractions.TestingHelpers.MockFileSystem();
            fs.AddFile("D:\\Plugins\\mockplugins.dll", new System.IO.Abstractions.TestingHelpers.MockFileData(assmBuffer));

            var conf = new MetadataSerializer().Deserialize<Configuration>("^plg1:\r\n  prop-two: 0.1\r\n  prp3:\r\n    - A\r\n    - B");
            conf.PluginsFolder = Location.FromPath("D:\\Plugins");
            conf.Plugins = new List<string>(new string[] { "plg1" });

            var mgr = new LocalFileSystemPluginsManager(conf, fs);

            var res = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mgr) as IEnumerable<IPlugin>;
            var plg = res.OfType<PluginMock<MockSettings1>>().FirstOrDefault();

            Assert.IsNotNull(plg);
            Assert.IsNotNull(plg.Settings);
            Assert.AreEqual("A", plg.Settings.Prp1);
            Assert.AreEqual(0.1, plg.Settings.PropTwo);
            Assert.That(new string[] { "A", "B" }.SequenceEqual(plg.Settings.Prp3));
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
            mgr.LoadPlugins(svcMock, false);

            var res1 = svcMock.GetType().GetField("m_Plugins1", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(svcMock) as IEnumerable<IPlugin1>;
            var res2 = svcMock.GetType().GetField("m_Plugins2", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(svcMock) as IEnumerable<IPlugin2>;

            Assert.AreEqual(2, res1.Count());
            Assert.AreEqual(2, res2.Count());
            Assert.That(res1.Contains(p1));
            Assert.That(res1.Contains(p3));
            Assert.That(res2.Contains(p2));
            Assert.That(res2.Contains(p3));
        }

        [Test]
        public void LoadPluginImportsTest()
        {
            var mgr = new LocalFileSystemPluginsManager(new Configuration());
            var field = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic);
            var p1 = new Plugin4();
            field.SetValue(mgr, new IPlugin[] { p1 });

            var svcMock = new ServiceMock2();
            mgr.LoadPlugins(svcMock, true);

            var res1 = p1.GetType().GetProperty("Service", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(p1) as ServiceMock2;

            Assert.AreEqual(svcMock, res1);
        }
    }
}
