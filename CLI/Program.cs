//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Threading.Tasks;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core;

namespace Xarial.Docify.CLI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var srcDir = args[0];
            var outDir = args[1];

            var engine = new DocifyEngine(srcDir, outDir);

            var loader = engine.Resove<ILoader>();
            var composer = engine.Resove<IComposer>();
            var compiler = engine.Resove<ICompiler>();

            var elems = loader.Load();

            var site = composer.ComposeSite(elems, "");

            await compiler.Compile(site);
        }
    }
}
