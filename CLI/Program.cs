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
            var siteUrl = args[2];

            var engine = new DocifyEngine(srcDir, outDir, siteUrl);

            await engine.Build();
        }
    }
}
