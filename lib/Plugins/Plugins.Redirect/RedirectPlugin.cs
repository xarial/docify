//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Data;
using System.Linq;
using Xarial.Docify.Lib.Plugins.Redirect.Properties;
using Xarial.Docify.Lib.Plugins.Common.Data;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;

namespace Xarial.Docify.Lib.Plugins.Redirect
{
    public class RedirectPlugin : IPlugin<RedirectPluginSettings>
    {
        private IDocifyApplication m_App;
        private RedirectPluginSettings m_Setts;

        private const string REDIRECT_FROM_PARAM_NAME = "redirect-from";
        private const string REDIRECT_TO_PARAM_NAME = "redirect-to";

        public void Init(IDocifyApplication app, RedirectPluginSettings setts)
        {
            m_App = app;
            m_Setts = setts;

            m_App.Compiler.PreCompile += OnPreCompile;
        }

        private Task OnPreCompile(ISite site)
        {
            TraversePages(site, site.MainPage, "/");

            return Task.CompletedTask;
        }

        private void TraversePages(ISite site, IPage parentPage, string curUrl)
        {
            var redirectsFrom = parentPage.Data.GetParameterOrDefault<IEnumerable<string>>(REDIRECT_FROM_PARAM_NAME);

            if (redirectsFrom?.Any() == true)
            {
                foreach (var redirectFrom in redirectsFrom)
                {
                    var redirectUrl = redirectFrom;

                    if (!redirectUrl.StartsWith("/"))
                    {
                        redirectUrl = curUrl + redirectUrl;
                    }

                    var parts = redirectUrl.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    var name = parts.Last();

                    var basePage = CreatePagesPath(site, parts.Take(parts.Length - 1).ToArray());

                    var data = new PluginMetadata();

                    if (basePage.SubPages.FirstOrDefault(
                        p => string.Equals(p.Name, name, StringComparison.CurrentCultureIgnoreCase)) == null)
                    {
                        var redirectPage = CreateRedirectPage(name, Guid.NewGuid().ToString(), data, curUrl);
                        basePage.SubPages.Add(redirectPage);
                    }
                    else
                    {
                        throw new PluginUserMessageException($"Cannot create redirect page at '{redirectUrl}' as this page already exists");
                    }
                }
            }

            for (int i = parentPage.SubPages.Count - 1; i >= 0; i--)
            {
                var page = parentPage.SubPages[i];

                var pageUrl = curUrl + page.Name + "/";

                var redirectTo = page.Data.GetParameterOrDefault<string>(REDIRECT_TO_PARAM_NAME);

                if (!string.IsNullOrEmpty(redirectTo))
                {
                    var redirectPage = CreateRedirectPage(page.Name, page.Id, page.Data, redirectTo);
                    redirectPage.SubPages.AddRange(page.SubPages);
                    parentPage.SubPages.Remove(page);
                    parentPage.SubPages.Insert(i, redirectPage);
                    page = redirectPage;
                }

                TraversePages(site, page, pageUrl);
            }
        }

        private IPage CreatePagesPath(ISite site, string[] parts)
        {
            IPage curPage = site.MainPage;

            foreach (var part in parts)
            {
                var page = curPage.SubPages.FirstOrDefault(
                    p => string.Equals(p.Name, part, StringComparison.CurrentCultureIgnoreCase));

                if (page == null)
                {
                    page = new PluginPage(part, "",
                        Guid.NewGuid().ToString(),
                        new PluginMetadata() { { "sitemap", false } }, null);

                    curPage.SubPages.Add(page);
                }

                curPage = page;
            }

            return curPage;
        }

        private IPage CreateRedirectPage(string name, string id, IMetadata data, string redirectTo)
        {
            var content = string.Format(Resources.redirect, redirectTo, m_Setts.WaitSeconds);

            if (data.ContainsKey("layout"))
            {
                data.Remove("layout");
            }

            data["sitemap"] = false;

            var redirectPage = new PluginPage(name, content, id, data);

            return redirectPage;
        }
    }
}
