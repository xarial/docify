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
    /// Represents the site of this job
    /// </summary>
    public interface ISite
    {
        /// <summary>
        /// Site host url
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Main page of the site
        /// </summary>
        IPage MainPage { get; }

        /// <summary>
        /// Available site layouts
        /// </summary>
        List<ITemplate> Layouts { get; }

        /// <summary>
        /// Available site includes
        /// </summary>
        List<ITemplate> Includes { get; }

        /// <summary>
        /// Configuration of the site
        /// </summary>
        IConfiguration Configuration { get; }
    }
}
