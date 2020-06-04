//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using System;

namespace Xarial.Docify.CLI.Options
{
    [Verb("library", HelpText = "Manages library")]
    public class LibraryOptions
    {
        [Option('i', "install", Required = false, HelpText = "Installs standard library")]
        public bool Install { get; set; }

        [Option('u', "check-updates", Required = false, HelpText = "Checks if library updates available")]
        public bool CheckForUpdates { get; set; }

        [Option('v', "version", Required = false, HelpText = "Version of the library to install. Latest is installed if not specified")]
        public Version Version { get; set; }
    }
}
