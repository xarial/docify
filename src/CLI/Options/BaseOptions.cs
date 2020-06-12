using CommandLine;
using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.CLI.Options
{
    public class BaseOptions
    {
        [Option('v', "verbose", Required = false, HelpText = "Specifies if verbose loggings should be enabled")]
        public bool Verbose { get; set; }
    }
}
