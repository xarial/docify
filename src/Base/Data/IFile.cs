//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Entity representing the file in the environment
    /// </summary>
    public interface IFile : IContent, IResource
    {
        /// <summary>
        /// Location of the file
        /// </summary>
        ILocation Location { get; }
    }
}
