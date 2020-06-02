//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    /// <summary>
    /// Represents HTML page of the site
    /// </summary>
    public interface IPage : ISheet, IAssetsFolder
    {
        /// <summary>
        /// Sub pages of this page
        /// </summary>
        List<IPage> SubPages { get; }
    }
}
