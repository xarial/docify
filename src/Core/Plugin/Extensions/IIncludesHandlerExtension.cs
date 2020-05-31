//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface IIncludesHandlerExtension
    {
        Task<string> ResolveInclude(string includeName, IMetadata md, IPage page);
    }

    public class IncludesHandlerExtension : IIncludesHandlerExtension
    {
        public event Func<string, IMetadata, IPage, Task<string>> RequestResolveInclude;

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
    }
}
