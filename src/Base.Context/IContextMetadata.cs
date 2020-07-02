//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Context
{
    /// <summary>
    /// Represents the metadata in the context of the include
    /// </summary>
    public interface IContextMetadata : IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// Get the value of existing property
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="prpName">Name of the property</param>
        /// <returns>Property value</returns>
        T Get<T>(string prpName);

        /// <summary>
        /// Gets the value of existing property or returns a default value if property doesn't exist
        /// </summary>
        /// <see cref="Get{T}(string)"/>
        T GetOrDefault<T>(string prpName);

        /// <summary>
        /// Attempts to read the value of the property
        /// </summary>
        /// <param name="val"></param>
        /// <see cref="Get{T}(string)"/>
        /// <returns>True if property is read, False if not</returns>
        bool TryGet<T>(string prpName, out T val);

        /// <summary>
        /// Deserializes metadata into the structure
        /// </summary>
        /// <typeparam name="T">Type of the structure</typeparam>
        /// <returns>Deserialized structure</returns>
        T ToObject<T>();
    }
}
