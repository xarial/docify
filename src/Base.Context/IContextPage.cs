//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IContextPage
    {
        string Url { get; }
        string FullUrl { get; }
        //string Name { get; }
        //string RawContent { get; }
        IContextMetadata Data { get; }
        IReadOnlyList<IContextPage> SubPages { get; }
        IReadOnlyList<IContextAsset> Assets { get; }
    }
}
