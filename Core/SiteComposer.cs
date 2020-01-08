//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Core.Base;

namespace Xarial.Docify.Core
{
    public class SiteComposer : IComposer
    {
        private const char PATH_SEPARATOR = '\\';

        public Site ComposeSite(IEnumerable<IElementSource> elements, string basePath, string baseUrl)
        {
            var pages = new Dictionary<string, Page>(
                StringComparer.CurrentCultureIgnoreCase);

            var rootPages = new List<Page>();

            if (elements?.Any() == true)
            {
                var srcPages = elements.Where(e => e.Type == ElementType_e.Page);

                //TODO: handle the duplicate key exception and rethrow
                var pagePerRelPath = srcPages.ToDictionary(
                    p => GetRelativePath(p.Path, basePath), p => p);

                foreach (var pageData in pagePerRelPath.OrderBy(
                    p => p.Key.Count(c => c.Equals(PATH_SEPARATOR))))
                {
                    var relPath = pageData.Key;
                    var pageSrc = pageData.Value;
                    var pathParts = relPath.Split(PATH_SEPARATOR).SkipLast(1).ToArray();

                    var url = "";

                    for (int i = 0; i < pathParts.Length; i++)
                    {
                        var isRoot = i == 0;
                        var isPage = i == pathParts.Length - 1;

                        var sep = (i != 0) ? "/" : "";
                        var thisUrl = $"{url}{sep}{pathParts[i]}";

                        Page page = null;

                        if (!pages.TryGetValue(thisUrl, out page))
                        {
                            string thisRawContent = null;
                            IReadOnlyDictionary<string, string> thisPageData = null;

                            if (isPage)
                            {
                                GetPageData(pageSrc, out thisPageData, out thisRawContent);
                            }
                            else
                            {
                                //TODO: implement default attributes and raw content for auto-pages
                            }

                            page = new Page(thisUrl, thisPageData, null, thisRawContent);
                            pages.Add(thisUrl, page);

                            if (isRoot)
                            {
                                rootPages.Add(page);
                            }

                            if (!string.IsNullOrEmpty(url))
                            {
                                pages[url].ChildrenList.Add(page);
                            }
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

            return new Site(baseUrl, null, rootPages);
        }

        private void GetPageData(IElementSource pageSrc, out IReadOnlyDictionary<string, string> data, out string rawContent)
        {
            //TODO: extract front matter
            data = null;
            rawContent = null;
        }

        private bool IsInFolder(string path, string folderPath)
        {
            return path.StartsWith(folderPath,
                StringComparison.CurrentCultureIgnoreCase);
        }

        private string GetRelativePath(string path, string basePath)
        {
            if (IsInFolder(path, basePath))
            {
                path = path.Substring(basePath.Length);
            }

            return path.Trim(PATH_SEPARATOR);
        }
    }
}
