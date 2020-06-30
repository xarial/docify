//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xarial.Docify.Base.Exceptions;

namespace Xarial.Docify.Base.Data
{
    public static class AssetsFolderExtension
    {
        public static IEnumerable<IAsset> GetAllAssets(this IAssetsFolder folder)
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

        public static IAssetsFolder FindFolder(this IAssetsFolder curFolder, ILocation path)
        {
            foreach (var curDir in path.Segments)
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

            return curFolder;
        }

        public static IAsset FindAsset(this IAssetsFolder folder, ILocation path)
        {
            if (!path.IsFile())
            {
                throw new BaseUserMessageException("Location is not a file");
            }

            var fileName = path.FileName;

            var curFolder = folder.FindFolder(path);

            var asset = curFolder.Assets.FirstOrDefault(a => string.Equals(a.FileName, fileName));

            if (asset == null)
            {
                throw new AssetNotFoundException(curFolder, fileName);
            }

            return asset;
        }
    }
}
