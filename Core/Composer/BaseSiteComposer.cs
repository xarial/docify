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
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

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

        private void ParseTextFile(ITextSourceFile src, out string rawContent,
            out Metadata data, out string layoutName) 
        {
            src.Parse(out rawContent, out data);

            layoutName = data.GetRemoveParameterOrDefault<string>(LAYOUT_VAR_NAME);
        }

        private Page CreatePageFromSourceOrDefault(ITextSourceFile src,
            Location loc, 
            IReadOnlyDictionary<string, Template> layoutsMap,
            Dictionary<IReadOnlyList<string>, Asset[]> assetsMap)
        {
            string rawContent = null;
            Metadata pageData = null;
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

            Asset[] assets;
            if (assetsMap.TryGetValue(page.Location.Path, out assets))
            {
                page.Assets.AddRange(assets);
            }

            return page;
        }

        public Site ComposeSite(IEnumerable<ISourceFile> files, string baseUrl)
        {
            if (files?.Any() == true)
            {
                GroupSourceFiles(files, 
                    out IEnumerable<ITextSourceFile> srcPages, 
                    out IEnumerable<ITextSourceFile> srcLayouts,
                    out IEnumerable<ITextSourceFile> srcIncludes, 
                    out IEnumerable<ITextSourceFile> srcTextAssets,
                    out IEnumerable<IBinarySourceFile> srcBinaryAssets);

                if (!srcPages.Any()) 
                {
                    throw new EmptySiteException();
                }

                var layouts = ParseLayouts(srcLayouts);
                var includes = ParseIncludes(srcIncludes);
                var assets = ParseAssets(srcTextAssets, srcBinaryAssets);
                var mainPage = ParsePages(srcPages, layouts, assets);

                var site = new Site(baseUrl, mainPage, m_Config);
                site.Layouts.AddRange(layouts.Values);
                site.Includes.AddRange(includes);
                site.Assets.AddRange(assets);

                return site;
            }
            else 
            {
                throw new EmptySiteException();
            }
        }

        private void GroupSourceFiles(IEnumerable<ISourceFile> files,
            out IEnumerable<ITextSourceFile> pages, 
            out IEnumerable<ITextSourceFile> layouts,
            out IEnumerable<ITextSourceFile> includes,
            out IEnumerable<ITextSourceFile> textAssets,
            out IEnumerable<IBinarySourceFile> binaryAssets) 
        {
            if (files.Any(f => f == null))
            {
                throw new NullReferenceException("Null reference source file is detected");
            }

            var textFiles = files.OfType<ITextSourceFile>();
            var binFiles = files.OfType<IBinarySourceFile>();

            var notSupported = files.Except(textFiles).Except(binFiles);

            if (notSupported.Any())
            {
                throw new UnsupportedSourceFileTypesException(notSupported);
            }

            layouts = textFiles
                .Where(f => string.Equals(f.Location.Root, LAYOUTS_FOLDER,
                StringComparison.CurrentCultureIgnoreCase));

            textFiles = textFiles.Except(layouts);

            includes = textFiles
                .Where(f => string.Equals(f.Location.Root, INCLUDES_FOLDER,
                StringComparison.CurrentCultureIgnoreCase));

            textFiles = textFiles.Except(includes);

            pages = textFiles.Where(e => IsPage(e));

            textFiles = textFiles.Except(pages);

            textAssets = textFiles;
            binaryAssets = binFiles;
        }

        private List<Asset> ParseAssets(IEnumerable<ITextSourceFile> textAssets,
            IEnumerable<IBinarySourceFile> binaryAssets) 
        {
            var assets = new List<Asset>();

            assets.AddRange(textAssets.Select(a => new TextAsset(a.Content, a.Location)));
            assets.AddRange(binaryAssets.Select(a => new BinaryAsset(a.Content, a.Location)));

            return assets;
        }

        private Dictionary<string, Template> ParseLayouts(IEnumerable<ITextSourceFile> layoutFiles) 
        {
            var layouts = new Dictionary<string, Template>(StringComparer.CurrentCultureIgnoreCase);

            var layoutSrcList = layoutFiles
                .ToList();

            while (layoutSrcList.Any()) 
            {
                var layoutName = Path.GetFileNameWithoutExtension(layoutSrcList.First().Location.FileName);
                CreateLayout(layouts, layoutSrcList, layoutName);
            }

            return layouts;
        }

        private List<Template> ParseIncludes(IEnumerable<ITextSourceFile> includeFiles) 
        {
            var usedIncludes = new List<string>();

            var includesSrcList = includeFiles
                .ToList();

            return includesSrcList.Select(s =>
            {
                string rawContent;
                Metadata data;
                string layoutName;
                ParseTextFile(s, out rawContent, out data, out layoutName);

                var name = Path.GetFileNameWithoutExtension(s.Location.FileName);

                if (usedIncludes.Contains(name, StringComparer.CurrentCultureIgnoreCase)) 
                {
                    throw new DuplicateTemplateException(name);
                }

                usedIncludes.Add(name);

                return new Template(name, rawContent, data);
            }).ToList();
        }

        private Template CreateLayout(Dictionary<string, Template> layouts, 
            List<ITextSourceFile> layoutsSrcList, string layoutName) 
        {
            //TODO: detect circular dependencies

            var layoutFile = layoutsSrcList.Find(
                l => string.Equals(Path.GetFileNameWithoutExtension(l.Location.FileName), 
                layoutName, 
                StringComparison.CurrentCultureIgnoreCase));

            if (layoutFile == null) 
            {
                throw new MissingLayoutException(layoutName);
            }

            string rawContent;
            Metadata data;
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

        private Page ParsePages(IEnumerable<ITextSourceFile> srcPages,
            IReadOnlyDictionary<string, Template> layouts,
            IReadOnlyList<Asset> assets)
        {
            //TODO: identify if any layouts are not in use

            var pageMap = new Dictionary<IReadOnlyList<string>, Page>(
                new LocationDictionaryComparer());
            
            var assetsMap = assets
                .GroupBy(a => a.Location.Path, new LocationDictionaryComparer())
                .ToDictionary(g => g.Key, g => g.ToArray(), new LocationDictionaryComparer());

            var mainSrcPage = srcPages.FirstOrDefault(p => p.Location.IsRoot && p.Location.IsIndexPage());

            if (mainSrcPage == null)
            {
                throw new SiteMainPageMissingException();
            }

            srcPages = srcPages.Except(new ITextSourceFile[] { mainSrcPage });

            var mainPage = CreatePageFromSourceOrDefault(mainSrcPage, new Location(""), layouts, assetsMap);

            pageMap.Add(new List<string>(), mainPage);

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
                            pagePath.ToArray()), layouts, assetsMap);

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

            return mainPage;
        }
    }
}
