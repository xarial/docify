//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Represents metadata collection
    /// </summary>
    public interface IMetadata : IDictionary<string, object>
    {
        /// <summary>
        /// Creates an instance of this metdata from the soure data
        /// </summary>
        /// <param name="data">Base data</param>
        /// <returns>Instance of this metdata</returns>
        IMetadata Copy(IDictionary<string, object> data);
    }
}
