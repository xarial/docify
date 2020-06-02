//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.CLI.Options
{
    [Verb("genman", HelpText = "Generate secure library manifest")]
    public class GenerateLibraryManifestOptions
    {
        [Option('l', "lib", Required = true, HelpText = "Path to the library")]
        public string LibraryPath { get; set; }

        [Option('v', "version", Required = true, HelpText = "Version of the library")]
        public string Version { get; set; }

        [Option('c', "cert", Required = true, HelpText = "Path to certificate to generate signature")]
        public string CertificatePath { get; set; }

        [Option('p', "pwd", Required = false, HelpText = "Certificate password if applicable")]
        public string CertificatePassword { get; set; }

        [Option('k', "pkey", Required = false, HelpText = "Path to the file to store public key for signature validation")]
        public string PublicKeyFile { get; set; }
    }
}
