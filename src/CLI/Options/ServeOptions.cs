//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using CommandLine;

namespace Xarial.Docify.CLI.Options
{
    [Verb("serve", HelpText = "Serves the static site from source")]
    public class ServeOptions : BuildOptions
    {
    }
}
