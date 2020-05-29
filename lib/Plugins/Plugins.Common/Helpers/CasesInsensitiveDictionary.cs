using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Lib.Plugins.Common.Helpers
{
    public class CasesInsensitiveDictionary<TValue> : Dictionary<string, TValue>
    {
        public CasesInsensitiveDictionary() : base(StringComparer.CurrentCultureIgnoreCase)
        {
        }
    }
}
