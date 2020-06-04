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
    /// Represents the configuration of the site, containing instruction for build, publish, library items, etc.
    /// </summary>
    public interface IConfiguration : IMetadata
    {
        /// <summary>
        /// Current environment
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// List of library components used in this site
        /// </summary>
        List<string> Components { get; set; }

        /// <summary>
        /// List of library plugins used in this site
        /// </summary>
        List<string> Plugins { get; set; }

        /// <summary>
        /// Themese hierarchy used for this site
        /// </summary>
        /// <remarks>Site can have a single theme, but theme itself can refer the theme, thus producing hierarchy</remarks>
        List<string> ThemesHierarchy { get; }
    }
}
