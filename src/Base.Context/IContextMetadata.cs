//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Context
{
    public interface IContextMetadata : IReadOnlyDictionary<string, object>
    {
        T Get<T>(string prpName);
        T GetOrDefault<T>(string prpName);
        bool TryGet<T>(string prpName, out T val);
    }
}
