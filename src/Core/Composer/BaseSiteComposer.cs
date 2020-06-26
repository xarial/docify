//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Helpers;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Composer
{
    public class BaseSiteComposer : IComposer
    {
        private const string LAYOUTS_FOLDER = "_layouts";
        private const string INCLUDES_FOLDER = "_includes";
        private const string LAYOUT_VAR_NAME = "layout";

        private const string INHERIT_PAGE_LAYOUT = "$";
        private const string DEFAULT_LAYOUT_PARAM_NAME = "default-layout";

        private readonly ILayoutParser m_LayoutParser;
        private readonly IConfiguration m_Config;

        private readonly StringComparer m_Comparer;
        private readonly StringComparison m_Comparison;

        private readonly string m_DefaultLayoutName;

        public BaseSiteComposer(ILayoutParser parser, IConfiguration config, IComposerExtension ext)
        {
            m_LayoutParser = parser;
            m_Config = config;
            m_Comparer = StringComparer.CurrentCultureIgnoreCase;
            m_Comparison = StringComparison.CurrentCultureIgnoreCase;

            m_DefaultLayoutName = m_Config?.GetParameterOrDefault<string>(DEFAULT_LAYOUT_PARAM_NAME);
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

        private void ParseTextFile(IFile src, out string rawContent,
            out IMetadata data, out string layoutName)
        {
            try
            {
                FrontMatterParser.Parse(src.AsTextContent(), out rawContent, out data);

                layoutName = data.GetRemoveParameterOrDefault<string>(LAYOUT_VAR_NAME);
            }
            catch (Exception ex)
            {
                throw new UserMessageException($"Failed to deserialize the metadata from the '{src.Location.ToPath()}'", ex);
            }
        }

        private bool IsDefaultPageLocation(ILocation location)
        {
            return Path.GetFileNameWithoutExtension(location.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        private Page CreatePage(IFile src,
            IReadOnlyDictionary<string, ITemplate> layoutsMap, string name, ITemplate defaultLayout)
        {
            string rawContent = null;
            IMetadata pageData = null;
            ITemplate layout = null;

            string layoutName;
            ParseTextFile(src, out rawContent, out pageData, out layoutName);

            if (string.IsNullOrEmpty(layoutName)) 
            {
                layoutName = m_DefaultLayoutName;
            }

            if (!string.IsNullOrEmpty(layoutName))
            {
                if (layoutName == INHERIT_PAGE_LAYOUT) 
                {
                    layout = defaultLayout;

                    if (defaultLayout == null) 
                    {
                        throw new MissingInheritLayoutException(src.Location.ToId(), INHERIT_PAGE_LAYOUT);
                    }
                }
                else if (!layoutsMap.TryGetValue(layoutName, out layout))
                {
                    throw new MissingLayoutException(layoutName);
                }
            }

            return new Page(name, rawContent, pageData, src.Id, layout);
        }

        public async Task<ISite> ComposeSite(IAsyncEnumerable<IFile> files, string baseUrl)
        {
            var filesList = new List<IFile>();

            await foreach (var file in files)
            {
                filesList.Add(file);
            }

            if (filesList.Any())
            {
                GroupSourceFiles(filesList,
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

        private void GroupSourceFiles(IReadOnlyList<IFile> files,
            out IEnumerable<IFile> pages,
            out IEnumerable<IFile> layouts,
            out IEnumerable<IFile> includes,
            out IEnumerable<IFile> assets)
        {
            if (files.Any(f => f == null))
            {
                throw new NullReferenceException("Null reference source file is detected");
            }

            var pagesList = new List<IFile>();
            var layoutsList = new List<IFile>();
            var includesList = new List<IFile>();
            var assetsList = new List<IFile>();

            foreach (var file in files)
            {
                if (string.Equals(file.Location.GetRoot(), LAYOUTS_FOLDER, m_Comparison))
                {
                    layoutsList.Add(file);
                }
                else if (string.Equals(file.Location.GetRoot(), INCLUDES_FOLDER, m_Comparison))
                {
                    includesList.Add(file);
                }
                else if (IsPage(file))
                {
                    pagesList.Add(file);
                }
                else
                {
                    assetsList.Add(file);
                }
            }

            layouts = layoutsList;
            includes = includesList;
            pages = pagesList;
            assets = assetsList;
        }

        private Dictionary<string, ITemplate> ParseLayouts(IEnumerable<IFile> layoutFiles)
        {
            var layouts = new Dictionary<string, ITemplate>(m_Comparer);

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

                return new Template(name, rawContent, s.Id, data);
            }).ToList();
        }

        private List<Data.File> ParseAssets(IEnumerable<IFile> assets)
        {
            return assets.Select(a => new Data.File(a.Location, a.Content, a.Id)).ToList();
        }

        private string GetTemplateName(ILocation loc)
        {
            var path = loc.Path.Skip(1).ToList();
            path.Add(Path.GetFileNameWithoutExtension(loc.FileName));

            return string.Join(LocationExtension.ID_SEP, path.ToArray());
        }

        private Template CreateLayout(Dictionary<string, ITemplate> layouts,
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

            try
            {
                m_LayoutParser.ValidateLayout(rawContent);
            }
            catch (Exception ex)
            {
                throw new InvalidLayoutException(layoutName, ex);
            }

            ITemplate baseLayout = null;

            if (!string.IsNullOrEmpty(baseLayoutName))
            {
                if (!layouts.TryGetValue(baseLayoutName, out baseLayout))
                {
                    baseLayout = CreateLayout(layouts, layoutsSrcList, baseLayoutName);
                }
            }

            if (baseLayout != null)
            {
                data = data.Merge(baseLayout.Data);
            }

            var layout = new Template(layoutName, rawContent, layoutFile.Id, data, baseLayout);

            if (layouts.ContainsKey(layoutName))
            {
                throw new DuplicateTemplateException(layoutName);
            }

            layouts.Add(layoutName, layout);
            layoutsSrcList.Remove(layoutFile);

            return layout;
        }

        private Page ParsePages(IEnumerable<IFile> srcPages,
            IReadOnlyDictionary<string, ITemplate> layouts,
            IReadOnlyList<IFile> assets)
        {
            var mainSrcPage = srcPages.FirstOrDefault(
                p => p.Location.IsRoot() && IsDefaultPageLocation(p.Location));

            if (mainSrcPage == null)
            {
                throw new SiteMainPageMissingException();
            }

            srcPages = srcPages.Except(new IFile[] { mainSrcPage });

            var mainPage = CreatePage(mainSrcPage, layouts, "", null);

            var refAssets = new List<IFile>(assets);
            var refPages = new List<IFile>(srcPages);

            ProcessChildren(mainPage, refPages, refAssets, layouts, Location.Empty, mainPage.Layout);

            var unprocessed = refPages.Concat(refAssets);

            if (unprocessed.Any())
            {
                //this should never happen, but keeping this as an exception in case algorithm is incorrect
                throw new SiteParsingException(unprocessed.ToArray());
            }

            return mainPage;
        }

        private void ProcessChildren(IPage parent,
            List<IFile> pages, List<IFile> assets,
            IReadOnlyDictionary<string, ITemplate> layouts, Location curLoc, ITemplate curLayout)
        {
            var subPages = pages.Where(p => p.Location.IsInLocation(curLoc, m_Comparison))
                .ToArray();

            var children = subPages.Where(p =>
            {
                var parentOffset = 1;
                if (IsDefaultPageLocation(p.Location))
                {
                    parentOffset = 2;
                }

                return p.Location.GetParent(parentOffset).IsSame(curLoc, m_Comparison);
            });

            foreach (var child in children)
            {
                pages.Remove(child);
            }

            void ProcessChildPage(IPage page)
            {
                parent.SubPages.Add(page);

                ProcessChildren(page, pages, assets, layouts,
                    new Location(curLoc.Path.Concat(new string[] { page.Name })), page.Layout ?? curLayout);
            }

            if(children.Any())
            {
                var usedNames = new List<string>();

                foreach (var child in children)
                {
                    var pageName = IsDefaultPageLocation(child.Location) ?
                        child.Location.Path.LastOrDefault()
                        : Path.GetFileNameWithoutExtension(child.Location.FileName);

                    if (usedNames.Contains(pageName,
                        m_Comparer))
                    {
                        throw new DuplicatePageException(child.Location);
                    }

                    usedNames.Add(pageName);

                    var page = CreatePage(child, layouts, pageName, curLayout);

                    if (IsDefaultPageLocation(child.Location))
                    {
                        ProcessChildPage(page);
                    }
                    else
                    {
                        parent.SubPages.Add(page);
                    }
                }
            }

            var phantomPages = subPages.Intersect(pages);

            if (phantomPages.Any())
            {
                foreach (var phantomGroup in phantomPages
                    .Select(p => p.Location.GetRelative(curLoc).GetRoot())
                    .Distinct(m_Comparer))
                {
                    var phantomPage = new PhantomPage(phantomGroup);

                    ProcessChildPage(phantomPage);
                }
            }

            if (!(parent is IPhantomPage))
            {
                LoadAssets(parent, assets, curLoc);
            }
        }

        private void LoadAssets(IAssetsFolder folder, List<IFile> assets, ILocation curLoc)
        {
            var pageAssets = assets.Where(a => a.Location.IsInLocation(curLoc, m_Comparison)).ToList();

            var children = pageAssets.Where(p => p.Location.GetParent().IsSame(curLoc, m_Comparison)).ToList();

            folder.Assets.AddRange(children.Select(a =>
            {
                var fileName = a.Location.FileName;

                if (string.IsNullOrEmpty(fileName))
                {
                    //file with no extension
                    fileName = a.Location.Path.Last();
                }

                return new Asset(fileName, a.Content, a.Id);
            }));

            children.ForEach(a => assets.Remove(a));
            children.ForEach(a => pageAssets.Remove(a));

            if (pageAssets.Any())
            {
                foreach (var subFolderName in pageAssets.Select(
                    a => a.Location.GetRelative(curLoc).GetRoot()).Distinct(m_Comparer))
                {
                    var subFolder = new AssetsFolder(subFolderName);
                    folder.Folders.Add(subFolder);
                    LoadAssets(subFolder, assets, curLoc.Combine(new Location("", subFolderName)));
                }
            }
        }
    }
}