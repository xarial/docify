using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IIncludeContextModel : IContextModel
    {
        IContextMetadata Data { get; }
    }
}
