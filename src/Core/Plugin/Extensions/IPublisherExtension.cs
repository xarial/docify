using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public struct PrePublishResult
    {
        public IFile File { get; set; }
        public bool SkipFile { get; set; }
    }

    public interface IPublisherExtension
    {
        Task PostPublish(ILocation loc);
        Task<PrePublishResult> PrePublishFile(ILocation outLoc, IFile file);
    }
}
