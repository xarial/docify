﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using CommandLine.Text;
using System.Collections.Generic;

namespace Xarial.Docify.CLI.Options
{
    [Verb("build", HelpText = "Builds the static site from source")]
    public class BuildOptions : BaseOptions
    {
        [Option('s', "src", Required = false, HelpText = "Source directories. If not specified, current directory is used")]
        public IEnumerable<string> SourceDirectories { get; set; }

        [Option('o', "out", Required = true, HelpText = "Output directory")]
        public virtual string OutputDirectory { get; set; }

        [Option('h', "host", Required = false, HelpText = "Target site host name")]
        public string Host { get; set; } = "";

        [Option('b', "base", Required = false, HelpText = "Base url")]
        public string BaseUrl { get; set; } = "";

        [Option('e', "env", Required = false, HelpText = "Build environment, either standard set or custom")]
        public string Environment { get; set; }

        [Option('l', "lib", Required = false, HelpText = "Path to libraries. For standard library specify " + DocifyEngine.STANDARD_LIB_PATH + ". For the folder based libraries specify the path to directory. For secure library specify the path to library manifest file and the public key XML file separated by pipe symbol " + DocifyEngine.LIB_PATH_PUBLIC_KEY_SEP)]
        public IEnumerable<string> Library { get; set; }

        [Usage]
        public static IEnumerable<Example> UsageExamples
        {
            get
            {
                return new List<Example>() 
                {
                    new Example("Compile the site source to the static files", 
                        new BuildOptions
                        {
                            SourceDirectories = new string[] { "C:\\my_site_src" },
                            Host = "https://www.example.com",
                            OutputDirectory = "C:\\my_site_compile"
                        }),

                    new Example("Compile the site from two sources using items from standard library in the Production environment",
                        new BuildOptions
                        {
                            SourceDirectories = new string[] { "C:\\my_site_src\\content", "C:\\my_site_src\\frame" },
                            Host = "https://www.example.com",
                            Library = new string[] { "*" },
                            Environment = "Production",
                            OutputDirectory = "C:\\my_site_compile"
                        }),

                    new Example("Compile the site using the local library signed with manifest",
                        new BuildOptions
                        {
                            SourceDirectories = new string[] { "C:\\my_site_src"  },
                            Host = "https://www.example.com",
                            Library = new string[] { "C:\\my_lib\\library.manifest|C:\\public_key.xml" },
                            OutputDirectory = "C:\\my_site_compile"
                        })
                };
            }
        }
    }
}
