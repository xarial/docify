//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public static class MetadataExtension
    {
        public static T Merge<T>(this T thisParams,
            Dictionary<string, dynamic> baseParams)
            where T : Metadata, new()
        {
            var resParams = new T();

            if (baseParams != null)
            {
                foreach (var baseParam in baseParams)
                {
                    resParams.Add(baseParam.Key, baseParam.Value);
                }
            }

            foreach (var thisParam in thisParams ?? new T())
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
