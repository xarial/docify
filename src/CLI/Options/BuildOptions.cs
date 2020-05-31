//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using System.Collections.Generic;

namespace Xarial.Docify.CLI.Options
{
    [Verb("build", HelpText = "Builds the static site from source")]
    public class BuildOptions
    {
        [Option('s', "src", Required = true, HelpText = "Source directories")]
        public IEnumerable<string> SourceDirectories { get; set; }

        [Option('o', "out", Required = true, HelpText = "Output directory")]
        public string OutputDirectory { get; set; }

        [Option('u', "url", Required = true, HelpText = "Target site url")]
        public string SiteUrl { get; set; }

        [Option('e', "env", HelpText = "Build environment, either standard set or custom", Required = false)]
        public string Environment { get; set; }

        [Option('l', "lib", HelpText = "Path to the library", Required = false)]
        public string Library { get; set; }
    }
}
