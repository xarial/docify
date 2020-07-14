//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Site : ISite
    {
        public string Host { get; }
        public string BaseUrl { get; }

        public IPage MainPage { get; }
        public List<ITemplate> Layouts { get; }
        public List<ITemplate> Includes { get; }
        public IConfiguration Configuration { get; }

        public Site(string host, string baseUrl, IPage mainPage, IConfiguration config)
        {
            Host = host;
            BaseUrl = baseUrl;

            MainPage = mainPage;

            Layouts = new List<ITemplate>();
            Includes = new List<ITemplate>();
            Configuration = config ?? new Configuration();
        }
    }
}
