//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Base
{
    public enum ElementType_e 
    {
        Page,
        Script,
        Style,
        Asset
    }

    public interface IElementSource
    {
        string Path { get; }
        string Content { get; }
        ElementType_e Type { get; }
    }
}
