//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

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
