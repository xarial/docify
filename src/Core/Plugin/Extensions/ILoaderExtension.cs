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
    public interface ILoaderExtension
    {
        Task PreLoadFile(PreLoadFileArgs args);
    }

    public class LoaderExtension : ILoaderExtension
    {
        public event PreLoadFileDelegate RequestPreLoadFile;

        public Task PreLoadFile(PreLoadFileArgs args)
        {
            if (RequestPreLoadFile != null)
            {
                return RequestPreLoadFile.Invoke(args);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
