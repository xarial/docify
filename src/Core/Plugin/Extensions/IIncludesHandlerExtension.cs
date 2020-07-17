//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface IIncludesHandlerExtension
    {
        Task<string> ResolveInclude(string includeName, IMetadata md, IPage page);
        Task PreResolveInclude(string includeName, IContextModel model);
        Task PostResolveInclude(string includeName, IContextModel model, StringBuilder html);
    }

    public class IncludesHandlerExtension : IIncludesHandlerExtension
    {
        public event Func<string, IMetadata, IPage, Task<string>> RequestResolveInclude;
        public event PreResolveIncludeDelegate RequestPreResolveInclude;
        public event PostResolveIncludeDelegate RequestPostResolveInclude;

        public Task<string> ResolveInclude(string includeName, IMetadata md, IPage page)
        {
            if (RequestResolveInclude != null)
            {
                return RequestResolveInclude.Invoke(includeName, md, page);
            }
            else
            {
                throw new Exception("Event for include resolve is not subscribed");
            }
        }

        public Task PreResolveInclude(string includeName, IContextModel model)
        {
            if (RequestPreResolveInclude != null)
            {
                return RequestPreResolveInclude.Invoke(includeName, model);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public Task PostResolveInclude(string includeName, IContextModel model, StringBuilder html)
        {
            if (RequestPostResolveInclude != null)
            {
                return RequestPostResolveInclude.Invoke(includeName, model, html);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
