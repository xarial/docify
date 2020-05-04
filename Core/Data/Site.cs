using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Site : ISite
    {
        public string BaseUrl { get; }

        //TODO: this needs to be removed as it is currently duplicated by page assets
        //public List<Asset> Assets { get; }

        public IPage MainPage { get; }
        public List<ITemplate> Layouts { get; }
        public List<ITemplate> Includes { get; }
        public IConfiguration Configuration { get; }

        public Site(string baseUrl, IPage mainPage, IConfiguration config)
        {
            BaseUrl = baseUrl;
            MainPage = mainPage;
            //Assets = new List<Asset>();
            Layouts = new List<ITemplate>();
            Includes = new List<ITemplate>();
            Configuration = config ?? new Configuration();
        }
    }
}
