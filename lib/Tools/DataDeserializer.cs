//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Tools
{
    public static class DataDeserializer
    {
        public static T Deserialize<T>(IContextMetadata data)
        {
            return MetadataExtension.ToObject<T>(new Dictionary<string, object>(data));
        }
    }
}
