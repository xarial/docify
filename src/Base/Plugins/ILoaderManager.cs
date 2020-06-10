using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    /// <summary>
    /// Arguments used in <see cref="ILoaderManager.PreLoadFile"/> event
    /// </summary>
    public class PreLoadFileArgs
    {
        /// <summary>
        /// File being pre-loaded
        /// </summary>
        public IFile File { get; set; }

        /// <summary>
        /// True to skip this file from loading, False to not
        /// </summary>
        public bool SkipFile { get; set; }
    }

    /// <summary>
    /// Delegate for <see cref="ILoaderManager.PreLoadFile"/> event
    /// </summary>
    /// <param name="args">Arguments of file preloading</param>
    public delegate Task PreLoadFileDelegate(PreLoadFileArgs args);

    /// <summary>
    /// Manages loading of project files
    /// </summary>
    public interface ILoaderManager
    {
        /// <summary>
        /// Fired when file is about to be loaded to project
        /// </summary>
        event PreLoadFileDelegate PreLoadFile;

        /// <summary>
        /// Instance of current file loader
        /// </summary>
        IFileLoader Instance { get; }
    }
}
