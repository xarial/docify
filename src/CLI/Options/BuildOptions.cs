//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.CLI.Options
{
    [Verb("build", HelpText = "Builds the static site from source")]
    public class BuildOptions
    {
        [Option('s', "src", Required = true, HelpText = "Source directoryies")]
        public IEnumerable<string> SourceDirectories { get; set; }

        [Option('o', "out", Required = true, HelpText = "Output directory")]
        public string OutputDirectory { get; set; }

        [Option('u', "url", Required = true, HelpText = "Target site url")]
        public string SiteUrl { get; set; }

        [Option('e', "env", HelpText = "Build environment, either standard set or custom",  Required = false)]
        public string Environment { get; set; }
    }
}
