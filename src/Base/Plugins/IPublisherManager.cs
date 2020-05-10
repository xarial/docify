﻿using System;
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

    public interface IPublisherManager
    {
        event PostPublishDelegate PostPublish;
        event PrePublishFileDelegate PrePublishFile;

        IPublisher Instance { get; }
    }
}
