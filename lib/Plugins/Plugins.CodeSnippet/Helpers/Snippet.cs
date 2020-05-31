//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Lib.Plugins.CodeSnippet.Helpers
{
    public class Snippet
    {
        public SnippetLocation_e Location { get; }
        public string Code { get; }

        public Snippet(string code, SnippetLocation_e location)
        {
            Location = location;
            Code = code;
        }
    }
}
