//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Context
{
    /// <summary>
    /// Representing site in the context of the include
    /// </summary>
    public interface IContextSite
    {
        /// <summary>
        /// Configuration of the site
        /// </summary>
        IContextConfiguration Configuration { get; }
        
        /// <summary>
        /// Base url
        /// </summary>
        string BaseUrl { get; }

        /// <summary>
        /// Entry page of the site
        /// </summary>
        IContextPage MainPage { get; }
    }
}
