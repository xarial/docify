using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Lib.Plugins.Data;
using Xarial.Docify.Lib.Plugins.Exceptions;

namespace Xarial.Docify.Lib.Plugins.Helpers
{
    public static class AssetsHelper
    {
        private static readonly string[] m_PathSeparators = new string[] { "\\", "/", "::" };
        internal static string[] PathSeparators => m_PathSeparators;

        public static IAsset FindAsset(ISite site, IAssetsFolder page, string path)
        {
            var isRel = !PathSeparators.Any(s => path.StartsWith(s));
            
            var parts = path.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);

            var dir = parts.Take(parts.Length - 1);
            var fileName = parts.Last();

            IAssetsFolder curFolder = null;

            if (isRel)
            {
                curFolder = page;
            }
            else 
            {
                curFolder = site.MainPage;
            }

            foreach (var curDir in dir) 
            {
                var nextDir = curFolder.Folders.FirstOrDefault(f => string.Equals(f.Name, curDir));

                if (nextDir == null) 
                {
                    if (curFolder is IPage) 
                    {
                        nextDir = (curFolder as IPage).SubPages.FirstOrDefault(p => string.Equals(p.Name, curDir));
                    }
                }

                if (nextDir == null) 
                {
                    throw new AssetNotFoundException(curFolder, curDir);
                }

                curFolder = nextDir;
            }

            var asset = curFolder.Assets.FirstOrDefault(a => string.Equals(a.FileName, fileName));

            if (asset == null) 
            {
                throw new AssetNotFoundException(curFolder, fileName);
            }

            return asset;
        }

        public static void AddTextAsset(string content, IPage page, string path)
            => AddAsset(ContentExtension.ToByteArray(content), page, path);

        public static void AddAsset(byte[] content, IPage page, string path)
        {
            var parts = path.Split(PathSeparators, StringSplitOptions.RemoveEmptyEntries);

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

        public static IEnumerable<IPage> GetAllPages(IPage page) 
        {
            yield return page;

            foreach (var childPage in page.SubPages) 
            {
                foreach (var subPage in GetAllPages(childPage)) 
                {
                    yield return subPage;
                }
            }
        }

        public static IEnumerable<IAsset> GetAllAssets(IAssetsFolder folder)
        {
            foreach (var asset in folder.Assets) 
            {
                yield return asset;
            }

            foreach (var subFolder in folder.Folders) 
            {
                foreach (var subAsset in GetAllAssets(subFolder)) 
                {
                    yield return subAsset;
                }
            }

            if (folder is IPage)
            {
                foreach (var subPage in (folder as IPage).SubPages) 
                {
                    foreach (var subPageAsset in GetAllAssets(subPage)) 
                    {
                        yield return subPageAsset;
                    }
                }
            }
        }
    }
}
