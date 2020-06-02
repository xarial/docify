//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

namespace Xarial.Docify.Base.Data
{
    /// <summary>
    /// Represents an entity which contains the user facing content (e.g. page or template)
    /// </summary>
    public interface ISheet : IResource
    {
        /// <summary>
        /// Raw content of the sheet
        /// </summary>
        /// <remarks>For example this can be a markdown text for page or razor page for template</remarks>
        string RawContent { get; }

        /// <summary>
        /// Parent layout of this sheet
        /// </summary>
        ITemplate Layout { get; }
        
        /// <summary>
        /// Metadata associated with this sheet
        /// </summary>
        IMetadata Data { get; }
    }
}
