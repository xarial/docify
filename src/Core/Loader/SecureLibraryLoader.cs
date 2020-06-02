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

        public IAsyncEnumerable<IFile> LoadComponentFiles(string componentName, string[] filters)
            => ProcessLibraryItems(Location.Library.ComponentsFolderName, componentName, m_Manifest.Components, filters);

        public IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters)
            => ProcessLibraryItems(Location.Library.PluginsFolderName, pluginId, m_Manifest.Plugins, filters);

        public IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters) 
            => ProcessLibraryItems(Location.Library.ThemesFolderName, themeName, m_Manifest.Themes, filters);

        private IAsyncEnumerable<IFile> ProcessLibraryItems(string itemType, string itemName, 
            SecureLibraryItem[] itemsList, string[] filters) 
        {
            var item = itemsList?.FirstOrDefault(i => string.Equals(i.Name, itemName, StringComparison.CurrentCultureIgnoreCase));

            if (item != null)
            {
                var libLoc = m_Loc.Combine(itemType, item.Name);

                try
                {
                    return LoadAndValidateFiles(libLoc, item.Files, filters);
                }
                catch (Exception ex)
                {
                    throw new LibraryItemLoadException(itemName, libLoc.ToId(), ex);
                }
            }
            else 
            {
                throw new UserMessageException($"'{itemName}' item is not present in the secure library manifest");
            }
        }

        public bool ContainsTheme(string themeName)
            => ContainsLibraryItem(m_Manifest.Themes, themeName);

        public bool ContainsComponent(string compName)
            => ContainsLibraryItem(m_Manifest.Components, compName);

        public bool ContainsPlugin(string pluginId)
            => ContainsLibraryItem(m_Manifest.Plugins, pluginId);

        private async IAsyncEnumerable<IFile> LoadAndValidateFiles(ILocation loc, SecureLibraryItemFile[] files, string[] filters)
        {
            await foreach (var file in m_FileLoader.LoadFolder(loc, filters))
            {
                var fileManifest = files.FirstOrDefault(f => file.Location.IsSame(f.Name));

                if (fileManifest != null)
                {
                    if (!m_Rsa.VerifyData(file.Content, fileManifest.Signature,
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
                    {
                        throw new DigitalSignatureMismatchException(file.Location);
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
