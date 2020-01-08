//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************/

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Base
{
    public interface IAssetSource 
    {
        string Path { get; }
    }

    public interface IBlobSource : IAssetSource
    {
        byte[] Array { get; }
    }

    public interface IScriptSource : IAssetSource
    {
        string Content { get; }
    }

    public interface IStyleSource : IAssetSource
    {
        string Content { get; }
    }
}
