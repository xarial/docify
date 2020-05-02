//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    public class Site
    {
        public string BaseUrl { get; }
        
        //TODO: this needs to be removed as it is currently duplicated by page assets
        //public List<Asset> Assets { get; }

        public Page MainPage { get; }
        public List<Template> Layouts { get; }
        public List<Template> Includes { get; }
        public Configuration Configuration { get; }

        public Site(string baseUrl, Page mainPage, Configuration config)
        {
            BaseUrl = baseUrl;
            MainPage = mainPage;
            //Assets = new List<Asset>();
            Layouts = new List<Template>();
            Includes = new List<Template>();
            Configuration = config ?? new Configuration();
        }
    }

    public static class SiteExtension 
    {
        public static IEnumerable<Page> GetAllPages(this Site site) 
        {
            yield return site.MainPage;

            foreach (var childPage in site.MainPage.GetAllSubPages()) 
            {
                yield return childPage;
            }
        }
    }
}
