using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IContextPage
    {
        string Url { get; }
        string FullUrl { get; }
        string Name { get; }
        string RawContent { get; }
        IContextMetadata Data { get; }
        IReadOnlyList<IContextPage> SubPages { get; }
        IReadOnlyList<IContextAsset> Assets { get; }
    }
}
