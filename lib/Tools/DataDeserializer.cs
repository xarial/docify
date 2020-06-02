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
    /// <summary>
    /// Metadata serialization helper
    /// </summary>
    public static class DataDeserializer
    {
        /// <summary>
        /// Deserializes metadata into the structure
        /// </summary>
        /// <typeparam name="T">Type of the structure</typeparam>
        /// <param name="data">Metadata</param>
        /// <returns>Deserialized structure</returns>
        public static T Deserialize<T>(IContextMetadata data)
        {
            return MetadataExtension.ToObject<T>(new Dictionary<string, object>(data));
        }
    }
}
