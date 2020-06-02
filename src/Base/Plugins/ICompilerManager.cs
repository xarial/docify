//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    public class PostCompileFileArgs 
    {
        public IFile File { get; set; }
    }

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.PreCompile"/> event
    /// </summary>
    /// <param name="site">Site which is about to be compiled</param>
    public delegate Task PreCompileDelegate(ISite site);

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.PreCompile"/> event
    /// </summary>
    /// <param name="rawCode">Raw code to be rendered</param>
    /// <param name="lang">Code language</param>
    /// <param name="args">Code user arguments</param>
    /// <param name="html">Reference html, contains current state of the code. Overwrite the content to provide a custom rendered representation</param>
    public delegate void RenderCodeBlockDelegate(string rawCode, string lang, string args, StringBuilder html);

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.RenderImage"/> event
    /// </summary>
    /// <param name="html">Current html for the image. Overwrite the content to provide new rendered representation</param>
    public delegate void RenderImageDelegate(StringBuilder html);

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.RenderUrl"/> event
    /// </summary>
    /// <param name="html">urrent html for the url. Overwrite the content to provide new rendered representation</param>
    public delegate void RenderUrlDelegate(StringBuilder html);

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.WritePageContent"/> event
    /// </summary>
    /// <param name="content"></param>
    /// <param name="data"></param>
    /// <param name="url"></param>
    /// <returns></returns>
    public delegate Task WritePageContentDelegate(StringBuilder content, IMetadata data, string url);

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.PostCompileFile"/> event
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public delegate Task PostCompileFileDelegate(PostCompileFileArgs args);

    /// <summary>
    /// Delegate for <see cref="ICompilerManager.PostCompile"/> event
    /// </summary>
    /// <returns></returns>
    public delegate Task PostCompileDelegate();

    /// <summary>
    /// Plugin interface for <see cref="ICompiler"/>
    /// </summary>
    public interface ICompilerManager
    {
        /// <summary>
        /// Raised before site started to compile
        /// </summary>
        event PreCompileDelegate PreCompile;

        /// <summary>
        /// Raised when code block is being rendered
        /// </summary>
        event RenderCodeBlockDelegate RenderCodeBlock;

        /// <summary>
        /// Raised when image is being rendered
        /// </summary>
        event RenderImageDelegate RenderImage;

        /// <summary>
        /// Raised when url is being rendered
        /// </summary>
        event RenderUrlDelegate RenderUrl;
        
        /// <summary>
        /// Raised when content is being written to the page
        /// </summary>
        event WritePageContentDelegate WritePageContent;

        /// <summary>
        /// Raised after file has been compiled
        /// </summary>
        event PostCompileFileDelegate PostCompileFile;

        /// <summary>
        /// Raised when compillation of all files and assets has been finished
        /// </summary>
        event PostCompileDelegate PostCompile;

        /// <summary>
        /// Instance of the compiler
        /// </summary>
        ICompiler Instance { get; }

        /// <summary>
        /// Instance of the transformer for the dynamic content (e.g. Razor pages)
        /// </summary>
        IDynamicContentTransformer DynamicContentTransformer { get; }

        /// <summary>
        /// Instance of the transformer for the static content (e.g. markdown)
        /// </summary>
        IStaticContentTransformer StaticContentTransformer { get; }
    }
}
