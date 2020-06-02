using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.XToolkit.Services.UserSettings;

namespace Xarial.Docify.Core.Publisher
{
    public class SecureLibraryCleaner : ITargetDirectoryCleaner
    {
        private readonly RSA m_Rsa;

        private readonly string m_ManifestFilePath;
        private readonly SecureLibraryManifest m_Manifest;
        private readonly IFileSystem m_FileSystem;

        public SecureLibraryCleaner(string manifestPath, string publicKeyXml) 
            : this(manifestPath, publicKeyXml, new FileSystem())
        {
        }

        public SecureLibraryCleaner(string manifestPath, string publicKeyXml, IFileSystem fileSystem)
        {
            m_FileSystem = fileSystem;

            if (m_FileSystem.File.Exists(manifestPath))
            {
                m_ManifestFilePath = manifestPath;

                m_Rsa = RSA.Create();
                m_Rsa.FromXmlString(publicKeyXml);
                
                using (var textReader = m_FileSystem.File.OpenText(manifestPath))
                {
                    m_Manifest = new UserSettingsService().ReadSettings<SecureLibraryManifest>(
                            textReader, new BaseValueSerializer<ILocation>(null, x => Location.FromString(x)));
                }                
            }
        }

        public async Task ClearDirectory(ILocation outDir)
        {
            if(m_Manifest != null)
            {
                var manifestDir = Path.GetDirectoryName(m_ManifestFilePath);

                if (string.Equals(manifestDir, outDir.ToPath(), StringComparison.CurrentCultureIgnoreCase))
                {
                    var dirs = new List<string>();
                    dirs.Add(manifestDir);

                    async Task RemoveLibraryItems(string itemsType, SecureLibraryItem[] items) 
                    {
                        var libItemDir = Path.Combine(manifestDir, itemsType);
                        dirs.Add(libItemDir);

                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                var itemDir = Path.Combine(libItemDir, item.Name);
                                dirs.Add(itemDir);

                                foreach (var file in item.Files)
                                {
                                    var filePath = Path.Combine(itemDir, file.Name.ToPath());

                                    var curFileDir = itemDir;

                                    foreach (var pathPart in file.Name.Path)
                                    {
                                        curFileDir = Path.Combine(curFileDir, pathPart);

                                        if (!dirs.Contains(curFileDir, StringComparer.CurrentCultureIgnoreCase))
                                        {
                                            dirs.Add(curFileDir);
                                        }
                                    }

                                    var buffer = await m_FileSystem.File.ReadAllBytesAsync(filePath);

                                    if (m_Rsa.VerifyData(buffer, file.Signature,
                                        HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
                                    {
                                        m_FileSystem.File.Delete(filePath);
                                    }
                                    else
                                    {
                                        throw new LibraryFileModifiedException(filePath);
                                    }
                                }
                            }
                        }
                    }

                    await RemoveLibraryItems(Location.Library.ComponentsFolderName, m_Manifest.Components);
                    await RemoveLibraryItems(Location.Library.ThemesFolderName, m_Manifest.Themes);
                    await RemoveLibraryItems(Location.Library.PluginsFolderName, m_Manifest.Plugins);

                    m_FileSystem.File.Delete(m_ManifestFilePath);

                    foreach (var dir in dirs.OrderByDescending(d => d.Count(c => c == '\\'))) 
                    {
                        if (!m_FileSystem.Directory.EnumerateFileSystemEntries(dir).Any())
                        {
                            m_FileSystem.Directory.Delete(dir);
                        }
                    }
                }
                else 
                {
                    throw new LibraryDirectoryManifestMismatchException(outDir.ToPath(), manifestDir);
                }
            }
        }
    }
}
