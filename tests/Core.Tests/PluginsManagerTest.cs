//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Tests.Common.Mocks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Helpers;
using Xarial.Docify.Core.Plugin;
using YamlDotNet.Serialization;

namespace Core.Tests
{
    public class PluginInfoMock : IPluginInfo
    {
        public string Name { get; }
        public IAsyncEnumerable<IFile> Files { get; }

        public PluginInfoMock(string name, params IFile[] files) 
        {
            Name = name;
            Files = files.ToAsyncEnumerable();
        }
    }

    public abstract class PluginMock<TSetts> : IPlugin<TSetts>
        where TSetts : new()
    {
        public IDocifyApplication App { get; private set; }
        public TSetts Settings { get; private set; }

        public void Init(IDocifyApplication app, TSetts setts)
        {
            App = app;
            Settings = setts;
        }
    }

    public abstract class PluginMock3 : IPlugin
    {
        public IDocifyApplication App { get; private set; }

        public void Init(IDocifyApplication app)
        {
            App = app;
        }
    }

    public class MockSettings1
    {
        public string Prp1 { get; set; } = "A";
        public double PropTwo { get; set; }
        public string[] Prp3 { get; set; }
    }

    public abstract class PluginMock1 : IPlugin
    {
        public IDocifyApplication App { get; private set; }

        public void Init(IDocifyApplication app)
        {
            App = app;
        }
    }

    public abstract class PluginMock2 : IPlugin
    {
        public IDocifyApplication App { get; private set; }

        public void Init(IDocifyApplication app)
        {
            App = app;
        }
    }

    public class PluginsManagerTest
    {   
        [Test]
        public async Task InitTest() 
        {
            var assmBuilder1 = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), 
                AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder1 = assmBuilder1.DefineDynamicModule(Guid.NewGuid().ToString());
            
            var typeBuilder1 = moduleBuilder1.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder1.AddInterfaceImplementation(typeof(IPlugin));
            typeBuilder1.SetParent(typeof(PluginMock1));
            var p1 = typeBuilder1.CreateType();
            
            var assm1 = moduleBuilder1.Assembly;
            
            var assmBuffer1 = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm1);
            
            var mgr = new PluginsManager(new Configuration() 
            {
                Plugins = new List<string>() 
            }, new Mock<IDocifyApplication>().Object);

            await mgr.LoadPlugins(new PluginInfoMock[] 
            {
                new PluginInfoMock("plg1", new FileMock(Location.FromPath("Plugin1Mock.dll"), assmBuffer1)),
            }.ToAsyncEnumerable());

            var res = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mgr) as IEnumerable<IPluginBase>;

            Assert.AreEqual(1, res.Count());
            Assert.IsInstanceOf(typeof(IPlugin), res.First());
            Assert.AreEqual(p1.FullName, res.First().GetType().FullName);
            Assert.IsNotNull((res.First() as PluginMock1).App);
        }

        [Test]
        public async Task LoadDefaultSettingsTest()
        {
            var assmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);

            var moduleBuilder = assmBuilder.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder = moduleBuilder.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IPlugin<MockSettings1>));
            typeBuilder.SetParent(typeof(PluginMock<MockSettings1>));
            typeBuilder.CreateType();

            var assm = moduleBuilder.Assembly;

            var assmBuffer = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm);

            var mgr = new PluginsManager(new Configuration()
            {
                Plugins = new string[] { "plugin1" }.ToList()
            }, new Mock<IDocifyApplication>().Object);

            await mgr.LoadPlugins(new PluginInfoMock[]
            {
                new PluginInfoMock("plugin1", new FileMock(Location.FromPath("mockplugins.dll"), assmBuffer))
            }.ToAsyncEnumerable());
                        
            var res = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mgr) as IEnumerable<IPluginBase>;
            var plg = res.OfType<PluginMock<MockSettings1>>().FirstOrDefault();

            Assert.IsNotNull(plg);
            Assert.IsNotNull(plg.Settings);
            Assert.AreEqual("A", plg.Settings.Prp1);
            Assert.AreEqual(0, plg.Settings.PropTwo);
            Assert.IsNull(plg.Settings.Prp3);
            Assert.IsNotNull(plg.App);
        }

        [Test]
        public async Task LoadSettingsTest()
        {
            var assmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);

            var moduleBuilder = assmBuilder.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder = moduleBuilder.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(typeof(IPlugin<MockSettings1>));
            typeBuilder.SetParent(typeof(PluginMock<MockSettings1>));
            typeBuilder.CreateType();

            var assm = moduleBuilder.Assembly;

            var assmBuffer = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm);

            var conf = new MetadataSerializer().Deserialize<Configuration>("^plg1:\r\n  prop-two: 0.1\r\n  prp3:\r\n    - A\r\n    - B");
            conf.Plugins = new List<string>(new string[] { "plg1" });

            var mgr = new PluginsManager(conf, new Mock<IDocifyApplication>().Object);

            await mgr.LoadPlugins(new PluginInfoMock[]
            {
                new PluginInfoMock("plg1", new FileMock(Location.FromPath("mockplugins.dll"), assmBuffer))
            }.ToAsyncEnumerable());
                        
            var res = mgr.GetType().GetField("m_Plugins", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(mgr) as IEnumerable<IPluginBase>;
            var plg = res.OfType<PluginMock<MockSettings1>>().FirstOrDefault();

            Assert.IsNotNull(plg);
            Assert.IsNotNull(plg.Settings);
            Assert.IsNotNull(plg.App);
            Assert.AreEqual("A", plg.Settings.Prp1);
            Assert.AreEqual(0.1, plg.Settings.PropTwo);
            Assert.That(new string[] { "A", "B" }.SequenceEqual(plg.Settings.Prp3));
        }

        [Test]
        public void PluginNameDuplicateTest() 
        {
            var assmBuilder1 = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder1 = assmBuilder1.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder1 = moduleBuilder1.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder1.AddInterfaceImplementation(typeof(IPlugin));
            typeBuilder1.SetParent(typeof(PluginMock1));
            var p1 = typeBuilder1.CreateType();

            var assm1 = moduleBuilder1.Assembly;

            var assmBuffer1 = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm1);

            var mgr = new PluginsManager(new Configuration()
            {
                Plugins = new List<string>()
            }, new Mock<IDocifyApplication>().Object);

            Assert.ThrowsAsync<DuplicatePluginException>(() => mgr.LoadPlugins(new PluginInfoMock[]
            {
                new PluginInfoMock("plg1", new FileMock(Location.FromPath("Plugin1Mock.dll"), assmBuffer1)),
                new PluginInfoMock("plg1", new FileMock(Location.FromPath("Plugin2Mock.dll"), assmBuffer1)),
            }.ToAsyncEnumerable()));
        }

        [Test]
        public void NoPluginFoundTest()
        {
            var assmBuilder1 = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder1 = assmBuilder1.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder1 = moduleBuilder1.DefineType("Plugin1", TypeAttributes.Public);
            var p1 = typeBuilder1.CreateType();

            var assm1 = moduleBuilder1.Assembly;

            var assmBuffer1 = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm1);

            var mgr = new PluginsManager(new Configuration()
            {
                Plugins = new List<string>()
            }, new Mock<IDocifyApplication>().Object);

            Assert.ThrowsAsync<MissingPluginImplementationException>(() => mgr.LoadPlugins(new PluginInfoMock[]
            {
                new PluginInfoMock("plg1", new FileMock(Location.FromPath("Plugin1Mock.dll"), assmBuffer1))
            }.ToAsyncEnumerable()));
        }

        [Test]
        public void MultipleInstancesPluginTest()
        {
            var assmBuilder1 = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()),
                AssemblyBuilderAccess.RunAndCollect);
            var moduleBuilder1 = assmBuilder1.DefineDynamicModule(Guid.NewGuid().ToString());

            var typeBuilder1 = moduleBuilder1.DefineType("Plugin1", TypeAttributes.Public);
            typeBuilder1.AddInterfaceImplementation(typeof(IPlugin));
            typeBuilder1.SetParent(typeof(PluginMock1));
            typeBuilder1.CreateType();

            var typeBuilder2 = moduleBuilder1.DefineType("Plugin2", TypeAttributes.Public);
            typeBuilder2.AddInterfaceImplementation(typeof(IPlugin));
            typeBuilder2.SetParent(typeof(PluginMock2));
            typeBuilder2.CreateType();

            var assm1 = moduleBuilder1.Assembly;

            var assmBuffer1 = new Lokad.ILPack.AssemblyGenerator().GenerateAssemblyBytes(assm1);

            var mgr = new PluginsManager(new Configuration()
            {
                Plugins = new List<string>()
            }, new Mock<IDocifyApplication>().Object);

            Assert.ThrowsAsync<MultiplePluginsPerNameException>(() => mgr.LoadPlugins(new PluginInfoMock[]
            {
                new PluginInfoMock("plg1", new FileMock(Location.FromPath("Plugin1Mock.dll"), assmBuffer1))
            }.ToAsyncEnumerable()));
        }
    }
}
