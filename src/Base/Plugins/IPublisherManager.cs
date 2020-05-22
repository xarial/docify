﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;

namespace Xarial.Docify.Base.Plugins
{
    public struct PrePublishResult
    {
        public IFile File { get; set; }
        public bool SkipFile { get; set; }
    }

    public delegate Task PostPublishDelegate(ILocation loc);
    public delegate Task<PrePublishResult> PrePublishFileDelegate(ILocation outLoc, IFile file);
    public delegate IAsyncEnumerable<IFile> PostAddPublishFilesDelegate(ILocation outLoc);
 
    public interface IPublisherManager
    {
        event PostAddPublishFilesDelegate PostAddPublishFiles;
        event PostPublishDelegate PostPublish;
        event PrePublishFileDelegate PrePublishFile;

        IPublisher Instance { get; }
    }
}
