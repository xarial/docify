//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Helpers;

namespace Xarial.Docify.Core.Composer
{
    public class BaseSiteComposer : IComposer
    {
        private const string LAYOUTS_FOLDER = "_layouts";
        private const string INCLUDES_FOLDER = "_includes";
        private const string LAYOUT_VAR_NAME = "layout";

        private readonly ILayoutParser m_LayoutParser;
        private readonly Configuration m_Config;
        
        public BaseSiteComposer(ILayoutParser parser, Configuration config) 
        {
            m_LayoutParser = parser;
            m_Config = config;
        }

        private bool IsPage(IFile srcFile) 
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

        private void ParseTextFile(IFile src, out string rawContent,
            out IMetadata data, out string layoutName) 
        {
            FrontMatterParser.Parse(src.AsTextContent(), out rawContent, out data);

            layoutName = data.GetRemoveParameterOrDefault<string>(LAYOUT_VAR_NAME);
        }

        private Page CreatePageFromSourceOrDefault(IFile src,
            ILocation loc, 
            IReadOnlyDictionary<string, Template> layoutsMap)
        {
            string rawContent = null;
            IMetadata pageData = null;
            Template layout = null;

            if (src != null)
            {
                string layoutName;
                ParseTextFile(src, out rawContent, out pageData, out layoutName);

                if (!string.IsNullOrEmpty(layoutName))
                {
                    if (!layoutsMap.TryGetValue(layoutName, out layout))
                    {
                        throw new MissingLayoutException(layoutName);
                    }
                }
            }
            else
            {
                //TODO: assign default attributes and content if available
            }
            
            var page = new Page(loc.ConvertToPageLocation(),
                rawContent, pageData, layout);

            return page;
        }

        public ISite ComposeSite(IEnumerable<IFile> files, string baseUrl)
        {
            if (files?.Any() == true)
            {
                GroupSourceFiles(files, 
                    out IEnumerable<IFile> srcPages, 
                    out IEnumerable<IFile> srcLayouts,
                    out IEnumerable<IFile> srcIncludes, 
                    out IEnumerable<IFile> srcAssets);

                if (!srcPages.Any()) 
                {
                    throw new EmptySiteException();
                }

                var layouts = ParseLayouts(srcLayouts);
                var includes = ParseIncludes(srcIncludes);
                var assets = ParseAssets(srcAssets);
                var mainPage = ParsePages(srcPages, layouts, assets);

                var site = new Site(baseUrl, mainPage, m_Config);
                site.Layouts.AddRange(layouts.Values);
                site.Includes.AddRange(includes);

                return site;
            }
            else 
            {
                throw new EmptySiteException();
            }
        }

        private void GroupSourceFiles(IEnumerable<IFile> files,
            out IEnumerable<IFile> pages, 
            out IEnumerable<IFile> layouts,
            out IEnumerable<IFile> includes,
            out IEnumerable<IFile> assets) 
        {
            if (files.Any(f => f == null))
            {
                throw new NullReferenceException("Null reference source file is detected");
            }

            var procFiles = files;

            layouts = procFiles
                .Where(f => string.Equals(f.Location.GetRoot(), LAYOUTS_FOLDER,
                StringComparison.CurrentCultureIgnoreCase));

            procFiles = procFiles.Except(layouts);

            includes = procFiles
                .Where(f => string.Equals(f.Location.GetRoot(), INCLUDES_FOLDER,
                StringComparison.CurrentCultureIgnoreCase));

            procFiles = procFiles.Except(includes);

            pages = procFiles.Where(e => IsPage(e));

            procFiles = procFiles.Except(pages);

            assets = procFiles;
        }
        
        private Dictionary<string, Template> ParseLayouts(IEnumerable<IFile> layoutFiles) 
        {
            var layouts = new Dictionary<string, Template>(StringComparer.CurrentCultureIgnoreCase);

            var layoutSrcList = layoutFiles.ToList();

            while (layoutSrcList.Any()) 
            {
                var layoutName = GetTemplateName(layoutSrcList.First().Location);
                CreateLayout(layouts, layoutSrcList, layoutName);
            }

            return layouts;
        }

        private List<Template> ParseIncludes(IEnumerable<IFile> includeFiles) 
        {
            var usedIncludes = new List<string>();

            var includesSrcList = includeFiles
                .ToList();

            return includesSrcList.Select(s =>
            {
                string rawContent;
                IMetadata data;
                string layoutName;
                ParseTextFile(s, out rawContent, out data, out layoutName);

                var name = GetTemplateName(s.Location);

                if (usedIncludes.Contains(name, StringComparer.CurrentCultureIgnoreCase)) 
                {
                    throw new DuplicateTemplateException(name);
                }

                usedIncludes.Add(name);

                return new Template(name, rawContent, data);
            }).ToList();
        }

        private List<Asset> ParseAssets(IEnumerable<IFile> assets) 
        {
            return assets.Select(a => new Asset(a.Location, a.Content)).ToList();
        }

        private string GetTemplateName(ILocation loc) 
        {
            var path = loc.Path.Skip(1).ToList();
            path.Add(Path.GetFileNameWithoutExtension(loc.FileName));
            
            return string.Join(Base.LocationExtension.ID_SEP, path.ToArray());
        }

        private Template CreateLayout(Dictionary<string, Template> layouts, 
            List<IFile> layoutsSrcList, string layoutName) 
        {
            //TODO: detect circular dependencies

            var layoutFile = layoutsSrcList.Find(
                l => string.Equals(GetTemplateName(l.Location), 
                layoutName, 
                StringComparison.CurrentCultureIgnoreCase));

            if (layoutFile == null) 
            {
                throw new MissingLayoutException(layoutName);
            }

            string rawContent;
            IMetadata data;
            string baseLayoutName;
            ParseTextFile(layoutFile, out rawContent, out data, out baseLayoutName);

            if (!m_LayoutParser.ContainsPlaceholder(rawContent)) 
            {
                throw new LayoutMissingContentPlaceholderException(layoutName);
            }

            Template baseLayout = null;

            if (!string.IsNullOrEmpty(baseLayoutName))
            {
                if (!layouts.TryGetValue(baseLayoutName, out baseLayout))
                {
                    baseLayout = CreateLayout(layouts, layoutsSrcList, baseLayoutName);
                }
            }

            var layout = new Template(layoutName, rawContent, data, baseLayout);

            if (layouts.ContainsKey(layoutName)) 
            {
                throw new DuplicateTemplateException(layoutName);
            }

            layouts.Add(layoutName, layout);
            layoutsSrcList.Remove(layoutFile);

            return layout;
        }

        private Page ParsePages(IEnumerable<IFile> srcPages,
            IReadOnlyDictionary<string, Template> layouts,
            IReadOnlyList<Asset> assets)
        {
            var refAssets = new List<Asset>(assets);

            //TODO: identify if any layouts are not in use

            var pageMap = new Dictionary<IReadOnlyList<string>, Page>(
                new LocationDictionaryComparer());
            
            var assetsMap = refAssets
                .GroupBy(a => a.Location.Path, new LocationDictionaryComparer())
                .ToDictionary(g => g.Key, g => g.ToArray(), new LocationDictionaryComparer());

            var mainSrcPage = srcPages.FirstOrDefault(p => p.Location.IsRoot() && p.Location.IsIndexPage());

            if (mainSrcPage == null)
            {
                throw new SiteMainPageMissingException();
            }

            srcPages = srcPages.Except(new IFile[] { mainSrcPage });

            var mainPage = CreatePageFromSourceOrDefault(mainSrcPage, new Location(""), layouts);

            pageMap.Add(new List<string>(), mainPage);

            foreach (var srcPage in srcPages.OrderBy(p => p.Location.GetTotalLevel()))
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

                    if (!pageMap.TryGetValue(thisLoc, out page))
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
                            pagePath.ToArray()), layouts);

                        Asset[] pageAssets;
                        if (assetsMap.TryGetValue(page.Location.Path, out pageAssets))
                        {
                            assetsMap.Remove(page.Location.Path);
                            page.Assets.AddRange(pageAssets);

                            foreach (var pageAsset in pageAssets)
                            {
                                refAssets.Remove(pageAsset);
                            }
                        }

                        pageMap.Add(thisLoc, page);

                        pageMap[parentLoc].SubPages.Add(page);
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

            mainPage.Assets.AddRange(assets);

            return mainPage;
        }
    }
}
