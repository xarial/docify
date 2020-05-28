using Autofac;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.CLI;

namespace CLI.Tests
{
    public class DocifyEngineMock : DocifyEngine
    {
        public IFileSystem FileSystem { get; private set; }

        public DocifyEngineMock(string srcDir, string outDir, string siteUrl, string env)
            : base(new string[] { srcDir }, outDir, "", siteUrl, env)
        {
        }

        protected override void RegisterDependencies(ContainerBuilder builder, string env)
        {
            base.RegisterDependencies(builder, env);
            FileSystem = new MockFileSystem();
            builder.RegisterInstance(FileSystem);
        }
    }
}
