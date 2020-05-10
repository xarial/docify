using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface IPublisherExtension
    {
        Task PostPublish(ILocation loc);
        Task<PrePublishResult> PrePublishFile(ILocation outLoc, IFile file);
    }

    public class PublisherExtension : IPublisherExtension
    {
        public event PostPublishDelegate RequestPostPublish;
        public event PrePublishFileDelegate RequestPrePublishFile;

        public Task PostPublish(ILocation loc)
        {
            return RequestPostPublish.Invoke(loc);
        }

        public Task<PrePublishResult> PrePublishFile(ILocation outLoc, IFile file)
        {
            return RequestPrePublishFile.Invoke(outLoc, file);
        }
    }
}
