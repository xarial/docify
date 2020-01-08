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
using System.Text.RegularExpressions;
using Xarial.Docify.Core.Base;
using Xarial.Docify.Core.Exceptions;
using YamlDotNet.Serialization;

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
        private const string LAYOUTS_FOLDER = "_layouts";
        private const string INCLUDES_FOLDER = "_includes";
        private const string FRONT_MATTER_HEADER = "---";
        private const string LAYOUT_VAR_NAME = "layout";

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

        private class LocationDictionaryComparer : IEqualityComparer<IReadOnlyList<string>>
        {
            private readonly StringComparison m_CompType;

            internal LocationDictionaryComparer(StringComparison compType = StringComparison.CurrentCultureIgnoreCase)
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

        private void ParseTextFile(ITextSourceFile src, out string content,
            out Dictionary<dynamic, dynamic> metadata, out string templateName) 
        {
            bool isStart = true;
            bool readingFrontMatter = false;

            content = "";
            var frontMatter = new StringBuilder();

            using (var strReader = new StringReader(src.Content))
            {
                var line = strReader.ReadLine();

                while(line != null)
                {
                    if (line == FRONT_MATTER_HEADER)
                    {
                        if (isStart)
                        {
                            readingFrontMatter = true;
                        }
                        else
                        {
                            readingFrontMatter = false;
                            content = strReader.ReadToEnd();
                        }
                    }
                    else if (readingFrontMatter)
                    {
                        frontMatter.AppendLine(line);
                    }
                    else
                    {
                        content = strReader.ReadToEnd();

                        content = line + (!string.IsNullOrEmpty(content) ? Environment.NewLine : "") + content;
                    }

                    isStart = false;
                    line = strReader.ReadLine();
                } 
            }

            if (readingFrontMatter) 
            {
                throw new FrontMatterErrorException("Front matter closing tag is not found");
            }

            if (frontMatter.Length > 0)
            {
                var yamlDeserializer = new DeserializerBuilder().Build();

                metadata = yamlDeserializer.Deserialize<Dictionary<dynamic, dynamic>>(frontMatter.ToString());

                var templateKey = metadata.FirstOrDefault(m => string.Equals(m.Key, LAYOUT_VAR_NAME, 
                    StringComparison.CurrentCultureIgnoreCase));

                metadata.Remove(templateKey);
                templateName = templateKey.Value;
            }
            else 
            {
                templateName = "";
                metadata = null;
            }
        }

        private Page CreatePageFromSourceOrDefault(ITextSourceFile src, Location loc) 
        {
            string rawContent = null;
            Dictionary<dynamic, dynamic> pageData = null;
            Template template = null;

            if (src != null)
            {
                //TODO: read front matter
                //TODO: remove from matter from the content
                //TODO: convert front matter to attributes

                string templateName;
                ParseTextFile(src, out rawContent, out pageData, out templateName);
            }
            else
            {
                //TODO: assign default attributes and content if available
            }

            //TODO: find template

            return new Page(loc.ConvertToPageLocation(),
                rawContent, pageData, template);
        }

        public Site ComposeSite(IEnumerable<ISourceFile> files, string baseUrl)
        {
            if (files?.Any() == true)
            {
                Dictionary<IReadOnlyList<string>, Page> pageMap = null;
                Site site = null;

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

                var srcPages = textFiles.Where(e => IsPage(e));

                if (!srcPages.Any()) 
                {
                    throw new EmptySiteException();
                }

                var templates = GetTemplates(textFiles);

                ParsePages(baseUrl, srcPages, out site, out pageMap);

                return site;
            }
            else 
            {
                throw new EmptySiteException();
            }
        }

        private Dictionary<string, Template> GetTemplates(IEnumerable<ITextSourceFile> textFiles) 
        {
            return textFiles.Where(f => string.Equals(f.Location.Root, LAYOUTS_FOLDER, StringComparison.CurrentCultureIgnoreCase))
                .Select(f => new Template(Path.GetFileNameWithoutExtension(f.Location.FileName), f.Content))
                .ToDictionary(t => t.Name);
        }

        private void ParsePages(string baseUrl, IEnumerable<ITextSourceFile> srcPages,
            out Site site, out Dictionary<IReadOnlyList<string>, Page> pageMap)
        {
            pageMap = new Dictionary<IReadOnlyList<string>, Page>(
                new LocationDictionaryComparer());

            var mainSrcPage = srcPages.FirstOrDefault(p => p.Location.IsRoot && p.Location.IsIndexPage());

            if (mainSrcPage == null)
            {
                throw new SiteMainPageMissingException();
            }

            srcPages = srcPages.Except(new ITextSourceFile[] { mainSrcPage });

            var mainPage = CreatePageFromSourceOrDefault(mainSrcPage, new Location(""));

            site = new Site(baseUrl, mainPage);
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
                            pagePath.ToArray()));

                        pageMap.Add(thisLoc, page);

                        pageMap[parentLoc].Children.Add(page);
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
    }
}
