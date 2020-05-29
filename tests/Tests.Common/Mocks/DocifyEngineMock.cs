using Autofac;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.CLI;

namespace Tests.Common.Mocks
{
    public class DocifyEngineMock : DocifyEngine
    {
        public IFileSystem FileSystem { get; private set; }

        public DocifyEngineMock()
            : base(new string[] { "D:\\src" }, "D:\\out", "", "www.xarial.com", "Test")
        {
        }

        protected override void RegisterDependencies(ContainerBuilder builder, string env)
        {
            base.RegisterDependencies(builder, env);
            FileSystem = new MockFileSystem();
            FileSystem.Directory.CreateDirectory("D:\\src");
            builder.RegisterInstance(FileSystem);
        }
    }
}
