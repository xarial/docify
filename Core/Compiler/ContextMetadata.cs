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

namespace Xarial.Docify.Core.Compiler
{
    public class ContextMetadata : ReadOnlyDictionary<string, dynamic>
    {
        public ContextMetadata(Metadata data) : base(data) 
        {
        }

        public T Get<T>(string prpName)
        {
            dynamic dynVal = this[prpName];
            
            if (dynVal is T)
            {
                return (T)dynVal;
            }
            else
            {
                return Convert.ChangeType(dynVal, typeof(T));
            }
        }
    }
}
