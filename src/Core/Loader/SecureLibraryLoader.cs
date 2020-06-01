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
            => ProcessLibraryItems(componentName, m_Manifest.Components);

        public IAsyncEnumerable<IFile> LoadPluginFiles(string pluginId, string[] filters)
            => ProcessLibraryItems(pluginId, m_Manifest.Plugins);

        public IAsyncEnumerable<IFile> LoadThemeFiles(string themeName, string[] filters) 
            => ProcessLibraryItems(themeName, m_Manifest.Themes);

        private IAsyncEnumerable<IFile> ProcessLibraryItems(string itemName, SecureLibraryItem[] itemsList) 
        {
            var item = itemsList?.FirstOrDefault(i => string.Equals(i.Name, itemName, StringComparison.CurrentCultureIgnoreCase));

            if (item != null)
            {
                var libLoc = m_Loc.Combine(item.Name);

                try
                {
                    return LoadAndValidateFiles(libLoc, item.Files);
                }
                catch (Exception ex)
                {
                    throw new LibraryItemLoadException(itemName, libLoc.ToId(), ex);
                }
            }
            else 
            {
                throw new Exception($"'{itemName}' item is not present in the secure library manifest");
            }
        }

        private async IAsyncEnumerable<IFile> LoadAndValidateFiles(ILocation loc, SecureLibraryItemFile[] files) 
        {
            await foreach (var file in m_FileLoader.LoadFolder(loc, null))
            {
                var fileRelLoc = file.Location.GetRelative(loc);

                var fileManifest = files.FirstOrDefault(f => fileRelLoc.IsSame(f.Name));

                if (fileManifest != null)
                {
                    if (!m_Rsa.VerifyData(file.Content, Convert.FromBase64String(fileManifest.Signature),
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
    }
}
