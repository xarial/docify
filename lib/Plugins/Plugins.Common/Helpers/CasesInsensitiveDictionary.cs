//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;

namespace Xarial.Docify.Lib.Plugins.Common.Helpers
{
    public class CasesInsensitiveDictionary<TValue> : Dictionary<string, TValue>
    {
        public CasesInsensitiveDictionary() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }
    }
}
