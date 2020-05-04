using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IContextAsset
    {
        byte[] Content { get; }
        string Name { get; }
    }
}
