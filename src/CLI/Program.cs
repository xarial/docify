//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.CLI.Options;

namespace Xarial.Docify.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var parser = new Parser(p =>
            {
                p.CaseInsensitiveEnumValues = true;
                p.AutoHelp = true;
                p.EnableDashDash = true;
                p.HelpWriter = Console.Out;
                p.IgnoreUnknownArguments = false;
            });

            bool isError = false;

            BuildOptions buildOpts = null;
            ServeOptions serveOpts = null;

            parser.ParseArguments<BuildOptions, ServeOptions>(args)
                .WithParsed<BuildOptions>(o => buildOpts = o)
                .WithParsed<ServeOptions>(o => serveOpts = o)
                .WithNotParsed(e => isError = true);

            if (!isError)
            {
                DocifyEngine engine = null;

                if (buildOpts != null || serveOpts != null)
                {
                    engine = new DocifyEngine(buildOpts.SourceDirectories.ToArray(),
                        buildOpts.OutputDirectory, buildOpts.Library, buildOpts.SiteUrl, buildOpts.Environment);
                }

                if (buildOpts != null)
                {
                    await engine.Build();
                }
            }
        }
    }
}
