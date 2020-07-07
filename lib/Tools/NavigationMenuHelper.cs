//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Context;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Lib.Tools.Exceptions;

namespace Xarial.Docify.Lib.Tools
{
    public class MenuPage : IContextPage
    {
        public string Url { get; set; }
        public string FullUrl { get; set; }
        public IContextMetadata Data { get; }
        public IReadOnlyList<IContextPage> SubPages => SubPagesList;
        public List<MenuPage> SubPagesList { get; }
        public Dictionary<string, object> DataDictionary { get; }
        public IReadOnlyList<IContextAsset> Assets => throw new NotSupportedException();
        public string[] Scope { get; set; }

        public MenuPage()
        {
            SubPagesList = new List<MenuPage>();
            DataDictionary = new Dictionary<string, object>();
            Data = new NavigationMenuHelper.MenuPageMetadata(DataDictionary);
        }
    }

    public class UrlLocation : ILocation
    {
        public string FileName { get; }
        public IReadOnlyList<string> Segments { get; }
        public string Root { get; }

        public UrlLocation(string url)
        {
            var parts = (IEnumerable<string>)url.Split(LocationExtension.URL_SEP);

            var fileName = "";

            if (!string.IsNullOrEmpty(System.IO.Path.GetExtension(parts.Last())))
            {
                parts = parts.Take(parts.Count() - 1);
                fileName = parts.Last();
            }

            FileName = fileName;
            Segments = new List<string>(parts);
        }

        private UrlLocation(string fileName, IEnumerable<string> path)
        {
            FileName = fileName;
            Segments = new List<string>(path);
        }

        public ILocation Create(string root, string fileName, IEnumerable<string> path)
        {
            return new UrlLocation(fileName, path);
        }
    }

    /// <summary>
    /// Helper classes for navigation menu composition
    /// </summary>
    public static class NavigationMenuHelper
    {
        private const string ROOT_PAGE_ATT = "root-page";

        internal class MenuPageMetadata : ReadOnlyDictionary<string, object>, IContextMetadata
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

            public T ToObject<T>() => MetadataExtension.ToObject<T>(this);
            public bool TryGet<T>(string prpName, out T val) => MetadataExtension.TryGetParameter<T>(this, prpName, out val);
        }

        /// <summary>
        /// Build the menu based on predefined parameter
        /// </summary>
        /// <param name="menuOptName">Name of the menu parameter</param>
        /// <param name="site">Site</param>
        /// <param name="data">Metadata or configuration</param>
        /// <returns>Predefined menu</returns>
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

        /// <summary>
        /// Creates new menu page
        /// </summary>
        /// <param name="srcPage">Source page</param>
        /// <param name="title">Page title</param>
        /// <returns>Menu page</returns>
        public static IContextPage BuildPage(IContextPage srcPage, string title)
        {
            var page = new MenuPage();
            page.DataDictionary["title"] = title;
            page.Url = srcPage.Url;
            page.FullUrl = srcPage.FullUrl;
            return page;
        }

        /// <summary>
        /// Finds the root page from this data
        /// </summary>
        /// <param name="model">Context model</param>
        /// <returns>Root page</returns>
        /// <remarks>Menu's root page can be defined with root-page attribute specifying the url of the root page</remarks>
        public static IContextPage GetRootPage(IContextModel model)
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
                    throw new RootPageNotFoundException(rootPageUrl);
                }
            }
            else
            {
                return model.Site.MainPage;
            }
        }

        public static IEnumerable<IContextPage> FilterChildrenPages(IEnumerable<IContextPage> pages, string[] filters)
        {
            return pages.Where(p => p is MenuPage || new UrlLocation(p.Url).Matches(filters));
        }

        private static void ParsePages(List<object> menuList, List<MenuPage> menuPagesList, IEnumerable<IContextPage> allPages, IContextMetadata data)
        {
            MenuPage CreateMenuPage(string val)
            {
                const string URL_PATTERN = @"\[(.*)\]\((.*)\)(?:{(.+)})?";

                var match = Regex.Match(val, URL_PATTERN);

                string url = "";
                string title = "";
                string[] scope = null;

                var overwriteTitle = match.Success;

                if (overwriteTitle)
                {
                    title = match.Groups[1].Value;
                    url = match.Groups[2].Value;
                    
                    var scopeAtt = match.Groups[3].Value;
                    
                    if (!string.IsNullOrEmpty(scopeAtt)) 
                    {
                        scope = scopeAtt.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(f => f.Trim()).ToArray();
                    }
                }
                
                var page = allPages?.FirstOrDefault(p => string.Equals(p.Url,
                    !string.IsNullOrEmpty(url) ? url : val, StringComparison.CurrentCultureIgnoreCase));

                if (page != null)
                {
                    url = page.Url;
                }
                else 
                {
                    if (!overwriteTitle) 
                    {
                        title = val;
                    }
                }
                
                var menuPage = new MenuPage()
                {
                    Url = url,
                    Scope = scope
                };

                menuPage.DataDictionary["title"] = val;

                if (page != null)
                {
                    menuPage.FullUrl = page.FullUrl;

                    foreach (var att in page.Data)
                    {
                        menuPage.DataDictionary[att.Key] = att.Value;
                    }
                }
                
                if (overwriteTitle) 
                {
                    menuPage.DataDictionary["title"] = title;
                    menuPage.DataDictionary["caption"] = title;
                }

                return menuPage;
            }
            
            foreach (var menuItem in menuList)
            {
                if (menuItem is string)
                {
                    menuPagesList.Add(CreateMenuPage((string)menuItem));
                }
                else if (menuItem is Dictionary<string, object>)
                {
                    foreach (var parentMenuItem in menuItem as Dictionary<string, object>)
                    {
                        var menuPage = CreateMenuPage(parentMenuItem.Key);
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
