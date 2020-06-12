//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;

namespace Xarial.Docify.CLI.Options
{
    [Verb("library", HelpText = "Manages library")]
    public class LibraryOptions : BaseOptions
    {
        [Option('i', "install", Required = false, HelpText = "Installs standard library")]
        public bool Install { get; set; }

        [Option('u', "check-updates", Required = false, HelpText = "Checks if library updates available")]
        public bool CheckForUpdates { get; set; }

        [Option('v', "version", Required = false, HelpText = "Version of the library to install. Latest is installed if not specified")]
        public Version Version { get; set; }

        [Usage]
        public static IEnumerable<Example> UsageExamples
        {
            get
            {
                return new List<Example>()
                {
                    new Example("Installs the latest version of standard library",
                        new LibraryOptions
                        {
                            Install = true
                        }),

                    new Example("Installs the standard library of version 0.1.0",
                        new LibraryOptions
                        {
                            Install = true,
                            Version = new Version("0.1.0")
                        })
                };
            }
        }
    }
}
