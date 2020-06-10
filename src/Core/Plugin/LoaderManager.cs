using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Plugin
{
    public class LoaderManager : ILoaderManager
    {
        public IFileLoader Instance { get; }

        public event PreLoadFileDelegate PreLoadFile;

        private readonly LoaderExtension m_Ext;

        public LoaderManager(IFileLoader loader, LoaderExtension ext) 
        {
            Instance = loader;
            m_Ext = ext;

            m_Ext.RequestPreLoadFile += OnRequestPreLoadFile;
        }

        private async Task OnRequestPreLoadFile(PreLoadFileArgs args)
        {
            if (PreLoadFile != null)
            {
                foreach (PreLoadFileDelegate del in PreLoadFile.GetInvocationList())
                {
                    var thisArg = new PreLoadFileArgs()
                    {
                        File = args.File,
                        SkipFile = false
                    };

                    await del.Invoke(thisArg);

                    args.File = thisArg.File;
                    args.SkipFile |= thisArg.SkipFile;
                }
            }
        }
    }
}
