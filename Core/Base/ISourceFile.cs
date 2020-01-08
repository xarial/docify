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
    public interface ISourceFile
    {
        Location Location { get; }
    }

    public interface IBinarySourceFile : ISourceFile
    {
        byte[] Content { get; }
    }

    public interface ITextSourceFile : ISourceFile
    {
        string Content { get; }
    }
}
