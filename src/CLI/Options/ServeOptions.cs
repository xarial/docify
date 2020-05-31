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
    }
}
