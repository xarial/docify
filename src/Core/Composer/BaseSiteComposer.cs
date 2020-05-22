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

        private readonly ILayoutParser m_LayoutParser;
        private readonly IConfiguration m_Config;

        private readonly StringComparer m_Comparer;
        private readonly StringComparison m_Comparison;

        public BaseSiteComposer(ILayoutParser parser, IConfiguration config, IComposerExtension ext) 
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

        private void ParseTextFile(IFile src, out string rawContent,
            out IMetadata data, out string layoutName) 
        {
            FrontMatterParser.Parse(src.AsTextContent(), out rawContent, out data);

            layoutName = data.GetRemoveParameterOrDefault<string>(LAYOUT_VAR_NAME);
        }

        private bool IsDefaultPageLocation(ILocation location)
        {
            return Path.GetFileNameWithoutExtension(location.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        private Page CreatePage(IFile src, 
            IReadOnlyDictionary<string, Template> layoutsMap, string name)
        {
            string rawContent = null;
            IMetadata pageData = null;
            Template layout = null;
            
            string layoutName;
            ParseTextFile(src, out rawContent, out pageData, out layoutName);

            if (!string.IsNullOrEmpty(layoutName))
            {
                if (!layoutsMap.TryGetValue(layoutName, out layout))
                {
                    throw new MissingLayoutException(layoutName);
                }
            }

            return new Page(name, rawContent, pageData, src.Id, layout);
        }

        public async Task<ISite> ComposeSite(IAsyncEnumerable<IFile> files, string baseUrl)
        {
            var filesList = new List<IFile>();

            await foreach(var file in files)
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

            IEnumerable<IFile> procFiles = files;

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
            IReadOnlyDictionary<string, Template> layouts,
            IReadOnlyList<IFile> assets)
        {
            var mainSrcPage = srcPages.FirstOrDefault(
                p => p.Location.IsRoot() && IsDefaultPageLocation(p.Location));

            if (mainSrcPage == null)
            {
                throw new SiteMainPageMissingException();
            }

            srcPages = srcPages.Except(new IFile[] { mainSrcPage });

            var mainPage = CreatePage(mainSrcPage, layouts, "");

            var refAssets = new List<IFile>(assets);
            var refPages = new List<IFile>(srcPages);

            ProcessChildren(mainPage, refPages, refAssets, layouts, Location.Empty);

            var unprocessed = refPages.Union(refAssets);

            if (unprocessed.Any()) 
            {
                //this should never happen, but keeping this as an exception in case algorithm is incorrect
                throw new SiteParsingException(unprocessed.ToArray());
            }
            
            return mainPage;
        }

        private void ProcessChildren(IPage parent, 
            List<IFile> pages, List<IFile> assets,
            IReadOnlyDictionary<string, Template> layouts, Location curLoc) 
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

                ProcessChildren(page, pages, assets, layouts, new Location(curLoc.Path.Union(new string[] { page.Name })));
            }

            if (!children.Any() && subPages.Any())
            {
                foreach (var phantomGroup in subPages
                    .Select(p => p.Location.GetRelative(curLoc).GetRoot())
                    .Distinct(m_Comparer))
                {
                    var phantomPage = new PhantomPage(phantomGroup);

                    ProcessChildPage(phantomPage);
                }
            }
            else
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

                    var page = CreatePage(child, layouts, pageName);

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

            if (!(parent is PhantomPage))
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
                    fileName = a.Location.GetRoot();
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
