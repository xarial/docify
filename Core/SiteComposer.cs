//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class SiteComposer : IComposer
    {
        private const string LAYOUTS_FOLDER = "_Layouts";
        private const string INCLUDES_FOLDER = "_Includes";
        
        private bool IsPage(ISourceFile srcFile) 
        {
            var ext = Path.GetExtension(srcFile.Path.FileName);

            var htmlExts = new string[]
            {
                ".html",
                ".md",
                ".cshtml"
            };

            return htmlExts.Contains(ext, StringComparer.CurrentCultureIgnoreCase);
        }

        private class PathDictionaryComparer : IEqualityComparer<IReadOnlyList<string>>
        {
            private readonly StringComparison m_CompType;

            internal PathDictionaryComparer(StringComparison compType = StringComparison.CurrentCultureIgnoreCase) 
            {
                m_CompType = compType;
            }

            public bool Equals([AllowNull] IReadOnlyList<string> x, [AllowNull] IReadOnlyList<string> y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode([DisallowNull] IReadOnlyList<string> obj)
            {
                return obj.GetHashCode();
            }
        }

        public Site ComposeSite(IEnumerable<ISourceFile> elements, string baseUrl)
        {
            var pages = new Dictionary<IReadOnlyList<string>, List<Page>>(
                new PathDictionaryComparer());

            var site = new Site(baseUrl);

            if (elements?.Any() == true)
            {
                var srcPages = elements.Where(e => IsPage(e));

                //TODO: handle the duplicate key exception and rethrow
                //var pagePerRelPath = srcPages.ToDictionary(
                //    p => p.Path, p => p);

                foreach (var srcPage in srcPages.OrderBy(p => p.Path.TotalLevel))
                {
                    var relPath = srcPage.Path;
                    
                    var url = new List<string>();

                    //var isIndexedPage = Path.GetFileNameWithoutExtension(relPath.FileName)
                    //    .Equals("index", StringComparison.CurrentCultureIgnoreCase);

                    for (int i = 0; i < relPath.Path.Count; i++)
                    {
                        var pathPart = relPath.Path[i];

                        var thisUrl = new List<string>(url);
                        thisUrl.Add(pathPart);

                        var isRoot = i == 0;
                        var isPage = i == relPath.Path.Count - 1;

                        List<Page> pagesList = null;

                        if (!pages.TryGetValue(thisUrl, out pagesList))
                        {
                            pagesList = new List<Page>();
                            pages.Add(thisUrl, pagesList);

                            string thisRawContent = null;
                            Dictionary<string, string> thisPageData = null;

                            if (isPage)
                            {
                                GetPageData(srcPage, out thisPageData, out thisRawContent);
                            }
                            else
                            {
                                //TODO: implement default attributes and raw content for auto-pages
                            }

                            var curPageUrl = new List<string>(thisUrl);

                            string pageName = "";

                            if (!isPage)
                            {
                                pageName = "index.html";
                            }
                            else 
                            {
                                pageName = Path.GetFileNameWithoutExtension(relPath.FileName) + ".html";
                            }

                            var page = new Page(new Location(pageName, curPageUrl.ToArray()),
                                thisRawContent, thisPageData);
                            pagesList.Add(page);
                            
                            if (isRoot)
                            {
                                site.Pages.Add(page);
                            }

                            //if (!url.IsEmpty)
                            //{
                            //    pages[url].Children.Add(page);
                            //}
                        }
                        else
                        {
                            if (isPage)
                            {
                                throw new Exception("Duplicate page");
                            }
                        }

                        url = thisUrl;
                    }
                }
            }
            else 
            {
                throw new Exception("Empty site");
            }

            return site;
        }

        private void GetPageData(ISourceFile pageSrc, 
            out Dictionary<string, string> data, out string rawContent)
        {
            //TODO: extract front matter
            data = null;
            rawContent = null;
        }
    }
}
