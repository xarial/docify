//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextModel : IContextModel
    {
        public IContextMetadata Data { get; }
        public IContextSite Site { get; }
        public IContextPage Page { get; }

        internal ContextModel(ISite site, IPage page, IMetadata data, string url)
        {
            Site = new ContextSite(site);
            Page = new ContextPage(site, page, url);
            Data = new ContextMetadata(data);
        }
    }
}
