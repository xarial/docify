//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
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
