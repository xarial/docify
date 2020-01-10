//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Xarial.Docify.Core.Composer
{
    internal class LocationDictionaryComparer : IEqualityComparer<IReadOnlyList<string>>
    {
        private readonly StringComparison m_CompType;

        internal LocationDictionaryComparer(StringComparison compType = StringComparison.CurrentCultureIgnoreCase)
        {
            m_CompType = compType;
        }

        public bool Equals([AllowNull] IReadOnlyList<string> x, [AllowNull] IReadOnlyList<string> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x is null || y is null)
            {
                return false;
            }

            if (x.Count == y.Count)
            {
                for (int i = 0; i < x.Count; i++)
                {
                    if (!string.Equals(x[i], y[i], m_CompType))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public int GetHashCode([DisallowNull] IReadOnlyList<string> obj)
        {
            return 0;
        }
    }
}
