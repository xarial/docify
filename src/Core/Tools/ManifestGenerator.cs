﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;

namespace Xarial.Docify.Core.Tools
{
    public class ManifestGenerator
    {
        private readonly IFileLoader m_Loader;

        public ManifestGenerator(IFileLoader loader) 
        {
            m_Loader = loader;
        }

        public Task<SecureLibraryManifest> CreateManifest(ILocation libFolder,
            Version vers, string certPath, string certificatePwd, out string publicKeyXml)
        {
            var cert = new X509Certificate2(certPath, certificatePwd);
            publicKeyXml = cert.GetRSAPublicKey().ToXmlString(false);

            var rsaWrite = cert.GetRSAPrivateKey();

            return CreateManifest(libFolder, rsaWrite, vers);
        }

        private async Task<SecureLibraryManifest> CreateManifest(ILocation libFolder, RSA rsaWrite, Version vers)
        {
            var components = new Dictionary<string, List<SecureLibraryItemFile>>(StringComparer.CurrentCultureIgnoreCase);
            var themes = new Dictionary<string, List<SecureLibraryItemFile>>(StringComparer.CurrentCultureIgnoreCase);
            var plugins = new Dictionary<string, List<SecureLibraryItemFile>>(StringComparer.CurrentCultureIgnoreCase);

            await foreach (var file in m_Loader.LoadFolder(libFolder, null))
            {
                if (file.Location.Path.Count >= 2)
                {
                    var itemType = file.Location.Path[0];
                    var itemName = file.Location.Path[1];
                    Dictionary<string, List<SecureLibraryItemFile>> thisComp = null;

                    switch (itemType.ToLower()) 
                    {
                        case Location.Library.ComponentsFolderName:
                            thisComp = components;
                            break;

                        case Location.Library.ThemesFolderName:
                            thisComp = themes;
                            break;

                        case Location.Library.PluginsFolderName:
                            thisComp = plugins;
                            break;

                        default:
                            continue;
                    }

                    List<SecureLibraryItemFile> files;

                    if (!thisComp.TryGetValue(itemName, out files)) 
                    {
                        files = new List<SecureLibraryItemFile>();
                        thisComp.Add(itemName, files);
                    }

                    var signature = rsaWrite.SignData(file.Content, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);

                    var fileManifest = new SecureLibraryItemFile()
                    {
                        Name = file.Location.GetRelative(new Location("", itemType, itemName)),
                        Signature = Convert.ToBase64String(signature)
                    };

                    files.Add(fileManifest);
                }
            }

            var manifest = new SecureLibraryManifest()
            {
                Version = vers,

                Components = components.Select(x => new SecureLibraryItem()
                {
                    Name = x.Key,
                    Files = x.Value.ToArray()
                }).ToArray(),

                Themes = themes.Select(x => new SecureLibraryItem()
                {
                    Name = x.Key,
                    Files = x.Value.ToArray()
                }).ToArray(),

                Plugins = plugins.Select(x => new SecureLibraryItem()
                {
                    Name = x.Key,
                    Files = x.Value.ToArray()
                }).ToArray()
            };

            return manifest;
        }
    }
}
