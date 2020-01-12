﻿//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Core.Compiler.Context;

namespace Components.Shared
{
    public static class NavigationMenuHelper
    {
        private const string NAME_ATT = "name_attribute";

        public class MenuPage : IContextPage
        {
            public string Url { get; set; } = "";
            public string FullUrl { get; set; }
            public string Name { get; }
            public ContextMetadata Data { get; }
            public IReadOnlyList<IContextPage> SubPages => SubPagesList;
            public List<MenuPage> SubPagesList { get; }

            public MenuPage(IContextPage page) : this(page.Name, page.Data)
            {
                FullUrl = page.FullUrl;
                Url = page.Url;
            }

            public MenuPage(string name, ContextMetadata data)
            {
                Name = name;
                SubPagesList = new List<MenuPage>();
                Data = data;
            }
        }

        public static IEnumerable<IContextPage> BuildPredefinedMenu(string menuOptName, ContextSite site, ContextMetadata data) 
        {
            var menu = data[menuOptName];

            if (menu != null)
            {
                var allPages = GetAllSubPages(site.MainPage);
                var menuPagesList = new List<MenuPage>();
                ParsePages(menu, menuPagesList, allPages, data);
                return menuPagesList;
            }
            else
            {
                return null;
            }
        }

        public static MenuPage BuildPage(ContextPage srcPage, ContextMetadata data, string title) 
        {
            var page = CreateMenuPage(null, data, title);
            page.Url = srcPage.Url;
            page.FullUrl = srcPage.FullUrl;
            return page;
        }
        
        public static string GetTitle(IContextPage page, ContextMetadata data)
        {
            var title = page.Data.Get<string>(data[NAME_ATT]);

            if (string.IsNullOrEmpty(title)) 
            {
                title = page.Name;
            }

            return title;
        }

        private static IEnumerable<IContextPage> GetAllSubPages(IContextPage page)
        {
            if (page.SubPages != null)
            {
                foreach (var childPage in page.SubPages)
                {
                    yield return childPage;

                    foreach (var subChildPage in GetAllSubPages(childPage))
                    {
                        yield return subChildPage;
                    }
                }
            }
        }

        private static MenuPage CreateMenuPage(IEnumerable<IContextPage> allPages, ContextMetadata data, string url)
        {
            var page = allPages?.FirstOrDefault(p => string.Equals(p.Url,
                url, StringComparison.CurrentCultureIgnoreCase));

            if (page != null)
            {
                return new MenuPage(page);
            }
            else
            {
                return new MenuPage(url, new ContextMetadata(
                    new Dictionary<string, dynamic>()
                    { { data[NAME_ATT] , url } }));
            }
        }

        private static void ParsePages(List<object> menuList, List<MenuPage> menuPagesList, IEnumerable<IContextPage> allPages, ContextMetadata data)
        {
            foreach (var menuItem in menuList)
            {
                if (menuItem is string)
                {
                    menuPagesList.Add(CreateMenuPage(allPages, data, (string)menuItem));
                }
                else if (menuItem is Dictionary<object, object>)
                {
                    foreach (var parentMenuItem in menuItem as Dictionary<object, object>)
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
