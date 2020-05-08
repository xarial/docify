//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Tools
{
    public static class NavigationMenuHelper
    {   
        private const string ROOT_PAGE_ATT = "root_page";

        public class MenuPage : IContextPage
        {
            public string Url { get; set; }
            public string FullUrl { get; set; }
            //public string Name { get; }
            public IContextMetadata Data { get; }
            public IReadOnlyList<IContextPage> SubPages => SubPagesList;
            public List<MenuPage> SubPagesList { get; }

            //public string RawContent { get; }

            public IReadOnlyList<IContextAsset> Assets => throw new NotSupportedException();

            public MenuPage(IContextPage page) : this(page.Data)
            {
                FullUrl = page.FullUrl;
                Url = page.Url;
            }

            public MenuPage(IContextMetadata data)
            {
                //Name = name;
                SubPagesList = new List<MenuPage>();
                Data = data;
            }
        }

        public class MenuPageMetadata : ReadOnlyDictionary<string, object>, IContextMetadata
        {
            public MenuPageMetadata(IDictionary<string, object> data) : base(data)
            {
            }

            public T Get<T>(string prpName)
            {
                T val;

                if (TryGet<T>(prpName, out val))
                {
                    return val;
                }
                else
                {
                    throw new KeyNotFoundException($"{prpName} is not present in the metadata");
                }
            }

            public T GetOrDefault<T>(string prpName)
            {
                T val;
                TryGet<T>(prpName, out val);

                return val;
            }

            public bool TryGet<T>(string prpName, out T val)
            {
                return MetadataExtension.TryGetParameter<T>(this, prpName, out val);
            }
        }

        public static IEnumerable<IContextPage> BuildPredefinedMenu(string menuOptName, IContextSite site, IContextMetadata data) 
        {
            List<object> menu;
            
            if (data.TryGet(menuOptName, out menu) && menu != null)
            {
                var allPages = PageHelper.GetAllPages(site.MainPage);
                var menuPagesList = new List<MenuPage>();
                ParsePages(menu, menuPagesList, allPages, data);
                return menuPagesList;
            }
            else
            {
                return null;
            }
        }

        public static MenuPage BuildPage(IContextPage srcPage, IContextMetadata data, string title) 
        {
            var page = CreateMenuPage(null, data, title);
            page.Url = srcPage.Url;
            page.FullUrl = srcPage.FullUrl;
            return page;
        }

        public static IContextPage GetRootPage(IIncludeContextModel model) 
        {
            var rootPageUrl = model.Data.GetOrDefault<string>(ROOT_PAGE_ATT);

            if (!string.IsNullOrEmpty(rootPageUrl))
            {

                var rootPage = PageHelper.GetAllPages(model.Site.MainPage).Prepend(model.Site.MainPage)
                    .FirstOrDefault(p => string.Equals(p.Url, rootPageUrl, StringComparison.CurrentCultureIgnoreCase));

                if (rootPage != null)
                {
                    return rootPage;
                }
                else
                {
                    throw new NullReferenceException("Specified root page is not found");
                }
            }
            else 
            {
                return model.Site.MainPage;
            }
        }

        private static MenuPage CreateMenuPage(IEnumerable<IContextPage> allPages, IContextMetadata data, string url)
        {
            var page = allPages?.FirstOrDefault(p => string.Equals(p.Url,
                url, StringComparison.CurrentCultureIgnoreCase));

            if (page != null)
            {
                return new MenuPage(page);
            }
            else
            {
                return new MenuPage(new MenuPageMetadata(
                    new Dictionary<string, object>()
                    { { "caption" , url } }));
            }
        }

        private static void ParsePages(List<object> menuList, List<MenuPage> menuPagesList, IEnumerable<IContextPage> allPages, IContextMetadata data)
        {
            foreach (var menuItem in menuList)
            {
                if (menuItem is string)
                {
                    menuPagesList.Add(CreateMenuPage(allPages, data, (string)menuItem));
                }
                else if (menuItem is Dictionary<string, object>)
                {
                    foreach (var parentMenuItem in menuItem as Dictionary<string, object>)
                    {
                        var menuPage = CreateMenuPage(allPages, data, (string)parentMenuItem.Key);
                        menuPagesList.Add(menuPage);
                        ParsePages(parentMenuItem.Value as List<object>, menuPage.SubPagesList, allPages, data);
                    }
                }
                else
                {
                    throw new NotSupportedException("Invalid type of menu item");
                }
            }
        }
    }
}
