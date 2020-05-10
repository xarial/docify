//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

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

        private readonly PublisherExtension m_Ext;

        public PublisherManager(IPublisher inst, PublisherExtension ext) 
        {
            Instance = inst;
            m_Ext = ext;

            m_Ext.RequestPostPublish += OnRequestPostPublish;
            m_Ext.RequestPrePublishFile += OnRequestPrePublishFile;
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
                    curRes.SkipFile &= thisRes.SkipFile;
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
    }
}
