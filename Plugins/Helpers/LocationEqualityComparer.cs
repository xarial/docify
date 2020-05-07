using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Lib.Plugins.Helpers
{
    public class LocationEqualityComparer : IEqualityComparer<ILocation>
    {
        public bool Equals([AllowNull] ILocation x, [AllowNull] ILocation y)
        {
            return x.IsSame(y);
        }

        public int GetHashCode([DisallowNull] ILocation obj)
        {
            return 0;
        }
    }
}
