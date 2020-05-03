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
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Data;

namespace Xarial.Docify.Core.Compiler.Context
{
    public interface IContextPage
    {
        string Url { get; }
        string FullUrl { get; }
        string Name { get; }
        string RawContent { get; }
        ContextMetadata Data { get; }
        IReadOnlyList<IContextPage> SubPages { get; }
        IReadOnlyList<IContextAsset> Assets { get; }
    }

    public class ContextPage : IContextPage
    {
        private readonly Site m_Site;

        internal Page BasePage { get; }

        public string Url => BasePage.Location.ToUrl();
        public string FullUrl => BasePage.Location.ToUrl(m_Site.BaseUrl);
        public string Name => BasePage.Location.FileName;
        public string RawContent => BasePage.RawContent;

        public ContextMetadata Data 
        {
            get
            {
                var thisParam = new Metadata();

                Frame frame = BasePage;

                while (frame != null) 
                {
                    thisParam = thisParam.Merge(frame.Data);
                    frame = frame.Layout;
                }

                return new ContextMetadata(thisParam);
            }
        }

        public IReadOnlyList<IContextPage> SubPages => BasePage.SubPages.ConvertAll(p => new ContextPage(m_Site, p));

        public IReadOnlyList<IContextAsset> Assets => BasePage.Assets
            .ConvertAll<IContextAsset>(a => new ContextAsset(a.Location.FileName, a.Content));

        public ContextPage(Site site, Page page) 
        {
            m_Site = site;
            BasePage = page;
        }
    }
}
