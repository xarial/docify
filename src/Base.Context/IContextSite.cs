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
        /// Generates full url using the host and base url
        /// </summary>
        string GetFullUrl(string url);

        /// <summary>
        /// Entry page of the site
        /// </summary>
        IContextPage MainPage { get; }
    }
}
