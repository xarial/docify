//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
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
        
        public IContextMetadata Data
        {
            get
            {
                var thisParam = new Metadata();

                ISheet frame = BasePage;

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
            .ConvertAll<IContextAsset>(a => new ContextAsset(a.FileName, a.Content));

        public ContextPage(ISite site, IPage page, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                url = LocationExtension.URL_SEP.ToString();
            }

            m_Site = site;
            BasePage = page;
            Url = url;
            FullUrl = site.GetFullUrl(url);
        }

        private string GetChildPageUrl(IPage page) => Url.TrimEnd(LocationExtension.URL_SEP) + LocationExtension.URL_SEP + page.Name + LocationExtension.URL_SEP;
    }
}
