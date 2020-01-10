//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Helpers
{
    internal static class ParametersHelper
    {
        internal static Dictionary<string, dynamic> MergeParameters(Dictionary<string, dynamic> thisParams,
            Dictionary<string, dynamic> baseParams)
        {
            var resParams = new Dictionary<string, dynamic>(
                baseParams ?? new Dictionary<string, dynamic>(), StringComparer.CurrentCultureIgnoreCase);

            foreach (var thisParam in thisParams ?? new Dictionary<string, dynamic>())
            {
                if (resParams.ContainsKey(thisParam.Key))
                {
                    resParams[thisParam.Key] = thisParam.Value;
                }
                else
                {
                    resParams.Add(thisParam.Key, thisParam.Value);
                }
            }

            return resParams;
        }
    }
}
