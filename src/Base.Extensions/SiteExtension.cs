using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base
{
    public static class SiteExtension
    {
        public static string GetFullUrl(this ISite site, string url)
        {
            var partUrl = url.TrimStart(LocationExtension.URL_SEP);

            var baseUrl = "";

            if (!string.IsNullOrEmpty(site.BaseUrl))
            {
                baseUrl = LocationExtension.URL_SEP + site.BaseUrl.TrimStart(LocationExtension.URL_SEP).TrimEnd(LocationExtension.URL_SEP);
            }

            if (!string.IsNullOrEmpty(partUrl))
            {
                partUrl = LocationExtension.URL_SEP + partUrl;
            }

            var host = "";

            if (!string.IsNullOrEmpty(site.Host))
            {
                host = site.Host.TrimEnd(LocationExtension.URL_SEP);
            }

            var res = host + baseUrl + partUrl;

            if (string.IsNullOrEmpty(res)) 
            {
                res = url;
            }

            return res;
        }
    }
}
