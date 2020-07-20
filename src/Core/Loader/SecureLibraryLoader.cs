//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class SecureLibraryLoader : ILibraryLoader
    {
        private readonly ILocation m_Loc;
        private readonly SecureLibraryManifest m_Manifest;
        private readonly IFileLoader m_FileLoader;
        private readonly ILogger m_Logger;
        private readonly RSA m_Rsa;

        public SecureLibraryLoader(ILocation loc, SecureLibraryManifest manifest,
            string publicKeyXml, IFileLoader fileLoader, ILogger logger)
        {
            m_Rsa = RSA.Create();
            m_Rsa.FromXmlString(publicKeyXml);

            m_Loc = loc;
            m_Manifest = manifest;

            m_FileLoader = fileLoader;
            m_Logger = logger;
        }

        public async IAsyncEnumerable<ILocation> EnumSubFolders(ILocation location)
        {
            await Task.CompletedTask;

            if (TryParseLibraryLocation(location, out string type, out string name, out ILocation path))
            {
                if (TryFindItem(type, name, out SecureLibraryItem item))
                {
                    foreach (var subFolder in item.Files
                        .Select(i => i.Name.Root)
                        .Where(i => !string.IsNullOrEmpty(i))
                        .Distinct(StringComparer.CurrentCultureIgnoreCase)) 
                    {
                        yield return location.Combine(subFolder);
                    }
                }
                else
                {
                    throw new UserMessageException($"'{type}/{name}' item is not present in the secure library manifest");
                }
            }
            else
            {
                throw new Exception("Invalid library item path");
            }
        }

        public bool Exists(ILocation location) => 
            TryParseLibraryLocation(location, out string type, out string name, out _) 
            && TryFindItem(type, name, out _);

        public IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters)
        {
            if (TryParseLibraryLocation(location, out string type, out string name, out ILocation path)) 
            {
                if (TryFindItem(type, name, out SecureLibraryItem item))
                {
                    var libLoc = m_Loc.Combine(location);

                    return LoadAndValidateFiles(libLoc, item.Files, filters);
                }
                else
                {
                    throw new UserMessageException($"'{type}/{name}' item is not present in the secure library manifest");
                }
            }
            else 
            {
                throw new Exception("Invalid library item path");
            }
        }

        private bool TryParseLibraryLocation(ILocation location, out string type, out string name, out ILocation path)
        {
            type = "";
            name = "";
            path = null;

            if (location.Segments.Count >= 2)
            {
                type = location.Segments[0];
                name = location.Segments[1];

                path = location.GetRelative(new Location(type, name, Enumerable.Empty<string>()));
                return true;
            }

            return false;
        }

        private bool TryFindItem(string type, string name, out SecureLibraryItem item)
        {
            item = null;

            SecureLibraryItem[] itemsList;

            if (string.Equals(type, Location.Library.ComponentsFolderName,
                StringComparison.CurrentCultureIgnoreCase))
            {
                itemsList = m_Manifest.Components;
            }
            else if (string.Equals(type, Location.Library.ComponentsFolderName,
                StringComparison.CurrentCultureIgnoreCase))
            {
                itemsList = m_Manifest.Components;
            }
            else if (string.Equals(type, Location.Library.ComponentsFolderName,
                StringComparison.CurrentCultureIgnoreCase))
            {
                itemsList = m_Manifest.Components;
            }
            else
            {
                return false;
            }

            item = itemsList.FirstOrDefault(i => string.Equals(
                i.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (item != null)
            {
                return true;
            }

            return false;
        }

        private async IAsyncEnumerable<IFile> LoadAndValidateFiles(
            ILocation loc, IEnumerable<SecureLibraryItemFile> files, string[] filters)
        {
            await foreach (var file in m_FileLoader.LoadFolder(loc, filters))
            {
                var fileManifest = files.FirstOrDefault(f => file.Location.IsSame(f.Name));

                if (fileManifest != null)
                {
                    if (!m_Rsa.VerifyData(file.Content, fileManifest.Signature,
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
                    {
                        throw new DigitalSignatureMismatchException(file.Location.ToId());
                    }

                    yield return file;
                }
                else
                {
                    m_Logger.LogWarning($"'{file.Location.ToId()}' is not listed in the library manifest and skipped");
                }
            }
        }

        private bool ContainsLibraryItem(SecureLibraryItem[] items, string itemName)
        {
            return items.FirstOrDefault(i => string.Equals(i.Name, itemName, StringComparison.CurrentCultureIgnoreCase)) != null;
        }
    }
}
