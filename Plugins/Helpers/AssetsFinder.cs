using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Lib.Plugins.Data;

namespace Xarial.Docify.Lib.Plugins.Helpers
{
    public static class AssetsFinder
    {
        private static readonly string[] m_Separators = new string[] { "\\", "/", "::" };

        public static IFile FindAsset(ISite site, IPage page, string path)
        {
            var isRel = !m_Separators.Any(s => path.StartsWith(s));
            
            var parts = path.Split(m_Separators, StringSplitOptions.RemoveEmptyEntries);

            var loc = new PluginDataFileLocation(parts.Last(), parts.Take(parts.Length - 1));

            if (isRel)
            {
                return page.FindAsset(loc);
            }
            else
            {
                return site.MainPage.FindAsset(loc, false);
            }
        }
    }
}
