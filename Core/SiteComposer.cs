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
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core
{
    internal static class LocationExtension
    {
        internal static bool IsIndexPage(this Location loc)
        {
            return Path.GetFileNameWithoutExtension(loc.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        internal static Location ConvertToPageLocation(this Location location)
        {
            var fileName = location.FileName;

            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "index.html";
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + ".html";
            }

            return new Location(fileName, location.Path.ToArray());
        }
    }

    public class SiteComposer : IComposer
    {
        private const string LAYOUTS_FOLDER = "_Layouts";
        private const string INCLUDES_FOLDER = "_Includes";
        
        private bool IsPage(ISourceFile srcFile) 
        {
            var ext = Path.GetExtension(srcFile.Location.FileName);

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

        private Page CreatePageFromSourceOrDefault(ISourceFile src, Location loc) 
        {
            string rawContent = null;
            Dictionary<string, string> pageData = null;
            Template template = null;

            if (src != null)
            {
                //TODO: read front matter
                //TODO: remove from matter from the content
                //TODO: convert front matter to attributes

                rawContent = src.Content;
                pageData = new Dictionary<string, string>();
            }
            else
            {
                //TODO: assign default attributes and content if available
            }

            //TODO: find template

            return new Page(loc.ConvertToPageLocation(),
                rawContent, pageData, template);
        }

        public Site ComposeSite(IEnumerable<ISourceFile> elements, string baseUrl)
        {
            var pages = new Dictionary<IReadOnlyList<string>, Page>(
                new PathDictionaryComparer());

            Site site = null;

            if (elements?.Any() == true)
            {
                var srcPages = elements.Where(e => IsPage(e));

                if (!srcPages.Any()) 
                {
                    throw new EmptySiteException();
                }

                var mainSrcPage = srcPages.FirstOrDefault(p => p.Location.IsRoot && p.Location.IsIndexPage());

                if (mainSrcPage == null) 
                {
                    throw new SiteMainPageMissingException();
                }

                srcPages = srcPages.Except(new ISourceFile[] { mainSrcPage });

                var mainPage = CreatePageFromSourceOrDefault(mainSrcPage, new Location(""));

                site = new Site(baseUrl, mainPage);
                pages.Add(new List<string>(), mainPage);
                
                foreach (var srcPage in srcPages.OrderBy(p => p.Location.TotalLevel))
                {
                    var pageLocParts = new List<string>();
                    pageLocParts.AddRange(srcPage.Location.Path);
                    var isIndexPage = srcPage.Location.IsIndexPage();
                    if (!isIndexPage) 
                    {
                        pageLocParts.Add(Path.GetFileNameWithoutExtension(srcPage.Location.FileName));
                    }

                    var parentLoc = new List<string>();

                    for (int i = 0; i < pageLocParts.Count; i++)
                    {
                        var pathPart = pageLocParts[i];

                        var thisLoc = new List<string>(parentLoc);
                        thisLoc.Add(pathPart);

                        var isPage = (i == pageLocParts.Count - 1);

                        Page page = null;

                        if (!pages.TryGetValue(thisLoc, out page))
                        {
                            List<string> pagePath = null;

                            if (isPage && !isIndexPage) 
                            {
                                pagePath = new List<string>(thisLoc.SkipLast(1));
                            }
                            else
                            {
                                pagePath = new List<string>(thisLoc);
                            }
                            
                            page = CreatePageFromSourceOrDefault(isPage ? srcPage : null,
                                new Location(isPage ? srcPage.Location.FileName : "",
                                pagePath.ToArray()));

                            pages.Add(thisLoc, page);
                            
                            pages[parentLoc].Children.Add(page);
                        }
                        else
                        {
                            if (isPage)
                            {
                                throw new DuplicatePageException(srcPage.Location);
                            }
                        }
                        
                        parentLoc = thisLoc;
                    }
                }
            }
            else 
            {
                throw new EmptySiteException();
            }

            return site;
        }
    }
}
