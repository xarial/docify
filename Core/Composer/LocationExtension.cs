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
using System.Text;
using Xarial.Docify.Base;

namespace Xarial.Docify.Core.Composer
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
                fileName = Base.LocationExtension.INDEX_PAGE_NAME;
            }
            else
            {
                fileName = Path.GetFileNameWithoutExtension(fileName) + ".html";
            }

            return new Location(fileName, location.Path.ToArray());
        }
    }
}
