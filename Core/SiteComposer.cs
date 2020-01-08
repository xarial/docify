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
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(null, x))
                {
                    return false;
                }

                if (ReferenceEquals(null, y))
                {
                    return false;
                }

                if (x.Count == y.Count)
                {
                    for (int i = 0; i < x.Count; i++) 
                    {
                        if (!string.Equals(x[i], y[i], m_CompType)) 
                        {
                            return false;
                        }
                    }
                }
                else 
                {
                    return false;
                }

                return true;
            }

            public int GetHashCode([DisallowNull] IReadOnlyList<string> obj)
            {
                return 0;
            }
        }

        private bool IsIndexPage(Location loc) 
        {
            return Path.GetFileNameWithoutExtension(loc.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        private Page CreatePageFromSourceOrDefault(ISourceFile src, IReadOnlyList<string> loc) 
        {
            string rawContent = null;
            Dictionary<string, string> pageData = null;
            Template template = null;

            if (src != null)
            {
                //TODO: read front matter
                //TODO: remove from matter from the content
                //TODO: convert front matter to attributes

                rawContent = "";
                pageData = new Dictionary<string, string>();
            }
            else
            {
                //TODO: assign default attributes and content if available
            }

            //TODO: find template

            var fileName = src?.Path.FileName;

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "index.html";
            }
            else 
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + ".html";
            }

            return new Page(new Location(fileName, loc.ToArray()),
                rawContent, pageData, template);
        }

        public Site ComposeSite(IEnumerable<ISourceFile> elements, string baseUrl)
        {
            var pages = new Dictionary<IReadOnlyList<string>, Page>(
                new PathDictionaryComparer());

            var site = new Site(baseUrl);

            if (elements?.Any() == true)
            {
                var srcPages = elements.Where(e => IsPage(e));
                
                var mainSrcPage = srcPages.FirstOrDefault(p => IsIndexPage(p.Path));

                if (mainSrcPage == null) 
                {
                    throw new Exception("Main page is missing");
                }

                srcPages = srcPages.Except(new ISourceFile[] { mainSrcPage });

                var mainPage = CreatePageFromSourceOrDefault(mainSrcPage, new List<string>());

                site.Pages.Add(mainPage);
                pages.Add(mainPage.Url.Path, mainPage);
                
                foreach (var srcPage in srcPages.OrderBy(p => p.Path.TotalLevel))
                {
                    var relPath = new Location(srcPage.Path.FileName, 
                        new string[] { "" }.Concat(srcPage.Path.Path).ToArray());
                    
                    var url = new List<string>();

                    var isIndexPage = IsIndexPage(srcPage.Path);

                    for (int i = 0; i < relPath.Path.Count; i++)
                    {
                        var pathPart = relPath.Path[i];

                        var thisUrl = new List<string>(url);
                        thisUrl.Add(pathPart);

                        var isPage = (i == relPath.Path.Count - 1);

                        Page page = null;

                        if (!pages.TryGetValue(url, out page) || (isPage && !isIndexPage))
                        {
                            page = CreatePageFromSourceOrDefault(isPage ? srcPage : null, thisUrl.Skip(1).ToList());

                            pages.Add(url, page);
                            
                            //var curPageUrl = new List<string>(thisUrl);

                            pages[url.Skip(1).ToList()].Children.Add(page);
                        }
                        //else
                        //{
                        //    if (isPage)
                        //    {
                        //        throw new Exception("Duplicate page");
                        //    }
                        //}

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

        //private void GetPageData(ISourceFile pageSrc, 
        //    out Dictionary<string, string> data, out string rawContent)
        //{
        //    //TODO: extract front matter
        //    data = null;
        //    rawContent = null;
        //}
    }
}
