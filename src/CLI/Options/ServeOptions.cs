//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;

namespace Xarial.Docify.CLI.Options
{
    [Verb("serve", HelpText = "Serves the static site from source")]
    public class ServeOptions : BuildOptions
    {
        [Option('o', "out", Required = false, HelpText = "Output directory. If not specified temp directory will be used")]
        public override string OutputDirectory { get; set; }

        [Option("http", Required = false, HelpText = "HTTP port of the served site (default 4080)")]
        public int HttpPort { get; set; } = 4080;

        [Option("https", Required = false, HelpText = "HTTPS port of the served site (default 4081)")]
        public int HttpsPort { get; set; } = 4081;
    }
}
