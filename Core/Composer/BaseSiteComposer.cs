﻿//*********************************************************************
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
        private readonly IConfiguration m_Config;

        private readonly StringComparer m_Comparer;
        private readonly StringComparison m_Comparison;

        public BaseSiteComposer(ILayoutParser parser, IConfiguration config) 
        {
            m_LayoutParser = parser;
            m_Config = config;
            m_Comparer = StringComparer.CurrentCultureIgnoreCase;
            m_Comparison = StringComparison.CurrentCultureIgnoreCase;
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

            return htmlExts.Contains(ext, m_Comparer);
        }

        private bool IsDefaultPage(IFile pageFile)
        {
            return Path.GetFileNameWithoutExtension(pageFile.Location.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        private void ParseTextFile(IFile src, out string rawContent,
            out IMetadata data, out string layoutName) 
        {
            FrontMatterParser.Parse(src.AsTextContent(), out rawContent, out data);

            layoutName = data.GetRemoveParameterOrDefault<string>(LAYOUT_VAR_NAME);
        }

        private Page CreatePage(IFile src, 
            IReadOnlyDictionary<string, Template> layoutsMap)
        {
            string rawContent = null;
            IMetadata pageData = null;
            Template layout = null;

            var pageLoc = new Location(
                Path.GetFileNameWithoutExtension(src.Location.FileName) + ".html", 
                src.Location.Path.ToArray());

            string layoutName;
            ParseTextFile(src, out rawContent, out pageData, out layoutName);

            if (!string.IsNullOrEmpty(layoutName))
            {
                if (!layoutsMap.TryGetValue(layoutName, out layout))
                {
                    throw new MissingLayoutException(layoutName);
                }
            }

            return new Page(pageLoc,
                rawContent, pageData, layout);
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
                m_Comparison));

            procFiles = procFiles.Except(layouts);

            includes = procFiles
                .Where(f => string.Equals(f.Location.GetRoot(), INCLUDES_FOLDER,
                m_Comparison));

            procFiles = procFiles.Except(includes);

            pages = procFiles.Where(e => IsPage(e));

            procFiles = procFiles.Except(pages);

            assets = procFiles;
        }
        
        private Dictionary<string, Template> ParseLayouts(IEnumerable<IFile> layoutFiles) 
        {
            var layouts = new Dictionary<string, Template>(m_Comparer);

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

                if (usedIncludes.Contains(name, m_Comparer)) 
                {
                    throw new DuplicateTemplateException(name);
                }

                usedIncludes.Add(name);

                return new Template(name, rawContent, data);
            }).ToList();
        }

        private List<Data.File> ParseAssets(IEnumerable<IFile> assets) 
        {
            return assets.Select(a => new Data.File(a.Location, a.Content)).ToList();
        }

        private string GetTemplateName(ILocation loc) 
        {
            var path = loc.Path.Skip(1).ToList();
            path.Add(Path.GetFileNameWithoutExtension(loc.FileName));
            
            return string.Join(LocationExtension.ID_SEP, path.ToArray());
        }

        private Template CreateLayout(Dictionary<string, Template> layouts, 
            List<IFile> layoutsSrcList, string layoutName) 
        {
            //TODO: detect circular dependencies

            var layoutFile = layoutsSrcList.Find(
                l => string.Equals(GetTemplateName(l.Location), 
                layoutName, 
                m_Comparison));

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
            IReadOnlyList<IFile> assets)
        {
            var mainSrcPage = srcPages.FirstOrDefault(p => p.Location.IsRoot() && IsDefaultPage(p));

            if (mainSrcPage == null)
            {
                throw new SiteMainPageMissingException();
            }

            srcPages = srcPages.Except(new IFile[] { mainSrcPage });

            var mainPage = CreatePage(mainSrcPage, layouts);

            var refAssets = new List<IFile>(assets);

            ProcessChildren(mainPage, srcPages, refAssets, layouts);

            System.Diagnostics.Debug.Assert(!refAssets.Any(), "Invalid algorithm, assets are not distributed across resolurces");
            
            return mainPage;
        }

        private void ProcessChildren(IPage parent, 
            IEnumerable<IFile> pages, List<IFile> assets,
            IReadOnlyDictionary<string, Template> layouts) 
        {
            var curLoc = parent.Location.GetParent();

            var children = pages.Where(p =>
            {
                var parentOffset = 1;
                if (IsDefaultPage(p)) 
                {
                    parentOffset = 2;
                }

                return p.Location.GetParent(parentOffset).IsSame(curLoc, m_Comparison);
            });

            void ProcessChildPage(IPage page) 
            {
                parent.SubPages.Add(page);
                var subPages = pages.Where(p => p.Location.IsInLocation(page.Location.GetParent(), m_Comparison));

                ProcessChildren(page, subPages, assets, layouts);
            }

            if (!children.Any() && pages.Any())
            {
                foreach (var phantomGroup in pages
                    .Select(p => p.Location.GetRelative(curLoc).GetRoot()) 
                    .Distinct(m_Comparer))
                {
                    var phantomPagePath = curLoc.Path.Union(new string[] { phantomGroup }).ToArray();

                    var phantomPage = new PhantomPage(
                        new Location("index.html", phantomPagePath));

                    ProcessChildPage(phantomPage);
                }
            }
            else
            {
                var usedNames = new List<string>();

                foreach (var child in children) 
                {
                    var pageName = IsDefaultPage(child) ?
                        child.Location.Path.LastOrDefault()
                        : Path.GetFileNameWithoutExtension(child.Location.FileName);

                    if (usedNames.Contains(pageName,
                        m_Comparer))
                    {
                        throw new DuplicatePageException(child.Location);
                    }
                    
                    usedNames.Add(pageName);

                    var page = CreatePage(child, layouts);

                    if (IsDefaultPage(child))
                    {
                        pages = pages.Except(new IFile[] { child });
                        ProcessChildPage(page);
                    }
                    else 
                    {
                        parent.SubPages.Add(page);
                    }
                }

                var pageAssets = assets.Where(a => a.Location.IsInLocation(curLoc, m_Comparison)).ToList();
                pageAssets.ForEach(a => assets.Remove(a));
                parent.Assets.AddRange(pageAssets);
            }
        }
    }
}
