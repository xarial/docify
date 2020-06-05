//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    public class PrePublishFileArgs
    {
        public IFile File { get; set; }
        public bool SkipFile { get; set; }
    }

    /// <summary>
    /// Delegate for the <see cref="IPublisherManager.PostPublish"/> event
    /// </summary>
    /// <param name="loc">Location whene files have been published</param>
    public delegate Task PostPublishDelegate(ILocation loc);

    /// <summary>
    /// Delegate for the <see cref="IPublisherManager.PrePublishFile"/> event
    /// </summary>
    /// <param name="outLoc">Location where files will be published</param>
    /// <param name="args">Publishing arguments</param>
    public delegate Task PrePublishFileDelegate(ILocation outLoc, PrePublishFileArgs args);

    /// <summary>
    /// Delegate for the <see cref="IPublisherManager.PostAddPublishFiles"/> event
    /// </summary>
    /// <param name="outLoc">Output location for publishing</param>
    /// <returns>New files to be added to publishing. Specify the absolute location for publishing</returns>
    public delegate IAsyncEnumerable<IFile> PostAddPublishFilesDelegate(ILocation outLoc);

    /// <summary>
    /// API service for <see cref="IPublisher"/>
    /// </summary>
    public interface IPublisherManager
    {
        /// <summary>
        /// Event raised when publishing of base files is finished allowing to provide additional files to publish
        /// </summary>
        event PostAddPublishFilesDelegate PostAddPublishFiles;

        /// <summary>
        /// Event is raise when publishing of all files is finished
        /// </summary>
        event PostPublishDelegate PostPublish;

        /// <summary>
        /// Event is raised before publishing is started
        /// </summary>
        event PrePublishFileDelegate PrePublishFile;

        /// <summary>
        /// Instance of current publisher
        /// </summary>
        IPublisher Instance { get; }
    }
}
