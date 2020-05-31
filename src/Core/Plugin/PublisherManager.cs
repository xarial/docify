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
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Plugin
{
    public class PublisherManager : IPublisherManager
    {
        public IPublisher Instance { get; }

        public event PostPublishDelegate PostPublish;
        public event PrePublishFileDelegate PrePublishFile;
        public event PostAddPublishFilesDelegate PostAddPublishFiles;
        private readonly PublisherExtension m_Ext;

        public PublisherManager(IPublisher inst, PublisherExtension ext)
        {
            Instance = inst;
            m_Ext = ext;

            m_Ext.RequestPrePublishFile += OnRequestPrePublishFile;
            m_Ext.RequestPostAddPublishFiles += OnRequestPostAddPublishFiles;
            m_Ext.RequestPostPublish += OnRequestPostPublish;
        }

        private async Task<PrePublishResult> OnRequestPrePublishFile(ILocation outLoc, IFile file)
        {
            var curRes = new PrePublishResult()
            {
                File = file,
                SkipFile = false
            };

            if (PrePublishFile != null)
            {
                foreach (PrePublishFileDelegate del in PrePublishFile.GetInvocationList())
                {
                    var thisRes = await del.Invoke(outLoc, curRes.File);
                    curRes.File = thisRes.File;
                    curRes.SkipFile |= thisRes.SkipFile;
                }
            }

            return curRes;
        }

        private async Task OnRequestPostPublish(ILocation loc)
        {
            if (PostPublish != null)
            {
                foreach (PostPublishDelegate del in PostPublish.GetInvocationList())
                {
                    await del.Invoke(loc);
                }
            }
        }

        private async IAsyncEnumerable<IFile> OnRequestPostAddPublishFiles(ILocation outLoc)
        {
            if (PostAddPublishFiles != null)
            {
                foreach (PostAddPublishFilesDelegate del in PostAddPublishFiles.GetInvocationList())
                {
                    await foreach (var file in del.Invoke(outLoc))
                    {
                        yield return file;
                    }
                }
            }
        }
    }
}
