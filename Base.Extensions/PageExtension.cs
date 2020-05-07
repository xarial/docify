using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base
{
    public static class PageExtension
    {
        public static bool IsDefaultPageLocation(ILocation location)
        {
            return Path.GetFileNameWithoutExtension(location.FileName)
                    .Equals("index", StringComparison.CurrentCultureIgnoreCase);
        }

        public static IEnumerable<IPage> GetAllSubPages(this IPage page)
        {
            if (page.SubPages != null)
            {
                foreach (var childPage in page.SubPages)
                {
                    yield return childPage;

                    foreach (var subChildPage in GetAllSubPages(childPage))
                    {
                        yield return subChildPage;
                    }
                }
            }
        }

        public static IPage FindPage(this IPage page, ILocation loc, bool isRelative = true) 
        {
            try
            {
                var curPage = TraversePages(page, loc, isRelative).Last();

                if (!IsDefaultPageLocation(loc))
                {
                    curPage = curPage.SubPages.FirstOrDefault(
                            p => string.Equals(p.Location.FileName,
                            loc.FileName, StringComparison.CurrentCultureIgnoreCase));
                }

                if (!string.Equals(curPage.Location.FileName,
                            loc.FileName, StringComparison.CurrentCultureIgnoreCase))
                {
                    throw new Exception("Invalid page is found");
                }

                return curPage;
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot find the '{loc.ToId()}' in '{page.Location.ToId()}'", ex);
            }
        }

        public static IFile FindAsset(this IPage page, ILocation loc, bool isRelative = true)
        {
            try 
            {
                var fullAssetPath = loc;

                if (isRelative)
                {
                    fullAssetPath = page.Location.Combine(loc);
                }

                foreach (var curPage in TraversePages(page, loc, isRelative)) 
                {
                    var asset = curPage.Assets.FirstOrDefault(a => a.Location.IsSame(fullAssetPath));
                    
                    if (asset != null) 
                    {
                        return asset;
                    }
                }

                throw new Exception("Asset is not found");
            }
            catch (Exception ex)
            {
                throw new Exception($"Cannot find the '{loc.ToId()}' asset in '{page.Location.ToId()}'", ex);
            }
        }

        private static IEnumerable<IPage> TraversePages(this IPage page, ILocation loc, bool isRelative = true)
        {
            if (!isRelative)
            {
                loc = loc.GetRelative(page.Location.GetParent());
            }

            if (!loc.IsFile())
            {
                throw new Exception($"'{loc.ToId()}' is not a file");
            }

            IPage curPage = page;
            yield return curPage;

            for (int i = 0; i < loc.Path.Count; i++)
            {
                //TODO: this might be improved to not create too many copies of location
                curPage = curPage.SubPages.First(p => string.Equals(
                            p.Location.GetRelative(page.Location.GetParent()).Path[i],
                            loc.Path[i], StringComparison.CurrentCultureIgnoreCase));

                yield return curPage;
            }
        }
    }
}
