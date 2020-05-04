using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Context
{
    public interface IContextMetadata : IReadOnlyDictionary<string, object>
    {
        T Get<T>(string prpName);
        T GetOrDefault<T>(string prpName);
        bool TryGet<T>(string prpName, out T val);
    }
}
