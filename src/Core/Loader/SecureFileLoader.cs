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
    public class SecureFileManifest
    {
        public ILocation Location { get; set; }
        public byte[] Signature { get; set; }
    }

    public class SecureFileLoader : IFileLoader
    {
        private readonly ILocation m_Loc;
        private readonly SecureFileManifest[] m_Manifest;
        private readonly IFileLoader m_FileLoader;
        private readonly ILogger m_Logger;
        private readonly RSA m_Rsa;

        public SecureFileLoader(ILocation loc, SecureFileManifest[] manifest,
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

            foreach (var subFolder in m_Manifest
                        .Select(i => i.Location.Root)
                        .Where(i => !string.IsNullOrEmpty(i))
                        .Distinct(StringComparer.CurrentCultureIgnoreCase))
            {
                yield return location.Combine(subFolder);
            }
        }

        public bool Exists(ILocation location) => m_Manifest.Any(f => f.Location.IsInLocation(location));

        public async IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters)
        {
            await foreach (var file in m_FileLoader.LoadFolder(m_Loc.Combine(location), filters))
            {
                var fileManifest = m_Manifest.FirstOrDefault(f => location.Combine(file.Location).IsSame(f.Location));

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
    }
}
