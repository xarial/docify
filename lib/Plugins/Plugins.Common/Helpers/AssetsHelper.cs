//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Lib.Plugins.Common.Data;
using Xarial.Docify.Lib.Plugins.Common.Exceptions;

namespace Xarial.Docify.Lib.Plugins.Common.Helpers
{
    public static class AssetsHelper
    {
        public static IAsset FindAsset(ISite site, IAssetsFolder page, string path)
        {
            var loc = PluginLocation.FromPath(path);
            var folder = GetBaseFolder(site, page, loc);
            return folder.FindAsset(loc);
        }

        public static IAssetsFolder GetBaseFolder(ISite site, IAssetsFolder page, ILocation loc) 
        {
            if (loc.IsRelative())
            {
                return page;
            }
            else
            {
                return site.MainPage;
            }
        }

        public static void AddTextAsset(string content, IPage page, string path)
            => AddAsset(ContentExtension.ToByteArray(content), page, path);

        public static void AddAsset(byte[] content, IPage page, string path)
        {
            var parts = path.Split(PluginLocation.PathSeparators, StringSplitOptions.RemoveEmptyEntries);

            IAssetsFolder curFolder = page;

            for (int i = 0; i < parts.Length - 1; i++)
            {
                var name = parts[i];

                var nextFolder = curFolder.Folders.FirstOrDefault(
                    f => string.Equals(f.Name, name, StringComparison.CurrentCultureIgnoreCase));

                if (nextFolder == null)
                {
                    nextFolder = new PluginAssetsFolder(name);
                    curFolder.Folders.Add(nextFolder);
                }

                curFolder = nextFolder;
            }

            curFolder.Assets.Add(new PluginAsset(content, parts.Last()));
        }

        public static void AddAssetsFromZip(byte[] zipBuffer, IPage page)
        {
            using (var zipStream = new MemoryStream(zipBuffer))
            {
                using (var archive = new ZipArchive(zipStream))
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (entry.Length > 0)
                        {
                            using (var entryStream = entry.Open())
                            {
                                var entryBuffer = new byte[entry.Length];
                                entryStream.Read(entryBuffer, 0, entryBuffer.Length);
                                AddAsset(entryBuffer, page, entry.FullName);
                            }
                        }
                    }
                }
            }
        }
    }
}
