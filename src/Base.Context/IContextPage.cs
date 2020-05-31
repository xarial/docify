//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Context
{
    public interface IContextPage
    {
        string Url { get; }
        string FullUrl { get; }
        IContextMetadata Data { get; }
        IReadOnlyList<IContextPage> SubPages { get; }
        IReadOnlyList<IContextAsset> Assets { get; }
    }
}
