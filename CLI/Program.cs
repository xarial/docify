﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************/

using System;
using Xarial.Docify.Core;

namespace Xarial.Docify.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var srcDir = args[0];
            var outDir = args[1];

            var loaderConfig = new LocalFileSystemLoaderConfig(srcDir);
            var compilerConfig = new MarkdownRazorCompilerConfig("");
            var publConfig = new LocalFileSystemPublisherConfig(outDir);

            var publisher = new LocalFileSystemPublisher(publConfig);

            var loader = new LocalFileSystemLoader(loaderConfig);
            var site = loader.Load();

            var compiler = new MarkdownRazorCompiler(compilerConfig, null, publisher);
            compiler.Compile(site);
        }
    }
}
