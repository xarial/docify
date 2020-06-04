//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Represents an asset in the site
    /// </summary>
    /// <remarks>This is a file which contains data, but not considered as an html page (e.g. JavaScript, Stylesheet, image)</remarks>
    public interface IAsset : IContent, IResource
    {
        /// <summary>
        /// Name of the asset file
        /// </summary>
        string FileName { get; }
    }
}
