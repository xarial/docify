//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Core.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextPage : IContextPage
    {
        private readonly ISite m_Site;

        internal IPage BasePage { get; }

        public string Url { get; }
        public string FullUrl { get; }
        //public string Name { get; }
        //public string RawContent { get; }

        public IContextMetadata Data 
        {
            get
            {
                var thisParam = new Metadata();

                IFrame frame = BasePage;

                while (frame != null) 
                {
                    thisParam = thisParam.Merge(frame.Data);
                    frame = frame.Layout;
                }

                return new ContextMetadata(thisParam);
            }
        }

        private IReadOnlyList<IContextPage> m_SubPages;

        public IReadOnlyList<IContextPage> SubPages => m_SubPages
            ?? (m_SubPages = BasePage.SubPages
                .ConvertAll(p => new ContextPage(m_Site, p, GetChildPageUrl(p))));

        public IReadOnlyList<IContextAsset> Assets => BasePage.Assets
            .ConvertAll<IContextAsset>(a => new ContextAsset(a.Name, a.Content));

        public ContextPage(ISite site, IPage page, string url)
        {
            m_Site = site;
            BasePage = page;
            Url = url;
            
            FullUrl = site.BaseUrl.TrimEnd('/') + "/" + Url.TrimStart('/');
            //Name = page.Name;
            //RawContent = page.RawContent;
        }

        private string GetChildPageUrl(IPage page) => Url.TrimEnd('/') + "/" + page.Name + "/";
    }
}
