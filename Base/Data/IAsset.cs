using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Data
{
    public interface IAsset : IContent
    {
        string Name { get; }
    }
}
