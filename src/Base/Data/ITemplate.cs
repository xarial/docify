//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Template used for <see cref="ISheet"/>
    /// </summary>
    public interface ITemplate : ISheet
    {
        /// <summary>
        /// Name of the template
        /// </summary>
        string Name { get; }
    }
}
