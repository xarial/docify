//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextMetadata : ReadOnlyDictionary<string, object>, IContextMetadata
    {
        internal ContextMetadata(IMetadata data) : base(data)
        {
        }

        public ContextMetadata(IDictionary<string, object> data) : base(data)
        {
        }

        public T Get<T>(string prpName)
        {
            T val;

            if (TryGet<T>(prpName, out val))
            {
                return val;
            }
            else
            {
                throw new KeyNotFoundException($"{prpName} is not present in the metadata");
            }
        }

        public T GetOrDefault<T>(string prpName)
        {
            T val;
            TryGet<T>(prpName, out val);

            return val;
        }

        public bool TryGet<T>(string prpName, out T val)
        {
            return MetadataExtension.TryGetParameter<T>(this, prpName, out val);
        }
    }
}
