//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface IPublisherExtension
    {
        Task PostPublish(ILocation loc);
        Task PrePublishFile(ILocation outLoc, PrePublishFileArgs args);
        IAsyncEnumerable<IFile> PostAddPublishFiles(ILocation outLoc);
    }

    public class PublisherExtension : IPublisherExtension
    {
        public event PostPublishDelegate RequestPostPublish;
        public event PrePublishFileDelegate RequestPrePublishFile;
        public event PostAddPublishFilesDelegate RequestPostAddPublishFiles;

        public Task PostPublish(ILocation loc)
        {
            if (RequestPostPublish != null)
            {
                return RequestPostPublish.Invoke(loc);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task PrePublishFile(ILocation outLoc, PrePublishFileArgs args)
        {
            if (RequestPrePublishFile != null)
            {
                return RequestPrePublishFile.Invoke(outLoc, args);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public IAsyncEnumerable<IFile> PostAddPublishFiles(ILocation outLoc)
        {
            if (RequestPostAddPublishFiles != null)
            {
                return RequestPostAddPublishFiles.Invoke(outLoc);
            }
            else
            {
                return Empty<IFile>();
            }
        }

        private async IAsyncEnumerable<T> Empty<T>()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
