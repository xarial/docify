//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;

namespace Xarial.Docify.Lib.Plugins.CodeSnippet.Helpers
{
    [Flags]
    public enum SnippetLocation_e
    {
        Middle = 0,
        Start = 1,
        End = 2,
        Full = Start | End
    }
}
