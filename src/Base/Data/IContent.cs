//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Entity which represents the content
    /// </summary>
    public interface IContent
    {
        /// <summary>
        /// Data buffer
        /// </summary>
        byte[] Content { get; }
    }
}
