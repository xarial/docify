//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextMetadata : ReadOnlyDictionary<string, dynamic>
    {
        internal ContextMetadata(Metadata data) : base(data) 
        {
        }

        public ContextMetadata(IDictionary<string, dynamic> data) : base(data)
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

        private bool TryGet<T>(string prpName, out T val) 
        {
            dynamic dynVal;

            if (this.TryGetValue(prpName, out dynVal))
            {
                if (dynVal is T)
                {
                    val = (T)dynVal;
                }
                else
                {
                    val = Convert.ChangeType(dynVal, typeof(T));
                }

                return true;
            }
            else
            {
                val = default(T);
                return false;
            }
        }
    }
}
