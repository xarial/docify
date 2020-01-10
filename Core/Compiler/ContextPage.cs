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
using Xarial.Docify.Core.Helpers;

namespace Xarial.Docify.Core.Compiler
{
    public class ContextPage
    {
        private readonly Site m_Site;

        internal Page BasePage { get; }

        public string Url
        {
            get
            {
                return BasePage.Location.ToUrl();
            }
        }

        public string FullUrl 
        {
            get 
            {
                return BasePage.Location.ToUrl(m_Site.BaseUrl);
            }
        }

        public string Name 
        {
            get 
            {
                return BasePage.Location.FileName;
            }
        }

        public IReadOnlyDictionary<string, dynamic> Data 
        {
            get
            {
                var thisParam = new Dictionary<string, dynamic>();

                Frame frame = BasePage;

                while (frame != null) 
                {
                    thisParam = ParametersHelper.MergeParameters(thisParam, frame.Data);
                    frame = frame.Layout;
                }

                return thisParam;
            }
        }

        public IReadOnlyList<ContextPage> SubPages
        {
            get
            {
                return BasePage.SubPages.ConvertAll(p => new ContextPage(m_Site, p));
            }
        }

        public ContextPage(Site site, Page page) 
        {
            m_Site = site;
            BasePage = page;
        }
    }
}
