//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.XToolkit.Services.UserSettings;

namespace Xarial.Docify.Core.Library
{
    public class AppCenterSecureLibraryInstaller : ILibraryInstaller
    {
        private readonly string m_LibInfoUrl;
        private readonly ILocation m_ManifestFileLocation;
        private readonly IPublisher m_DestWriter;
        private readonly ILogger m_Logger;
        private readonly RSA m_Rsa;

        public AppCenterSecureLibraryInstaller(string libInfoUrl,
            ILocation manifestFileLoc, string publicKeyXml, IPublisher destWriter, ILogger logger) 
        {
            m_LibInfoUrl = libInfoUrl;
            m_ManifestFileLocation = manifestFileLoc;
            m_DestWriter = destWriter;
            m_Logger = logger;

            m_Rsa = RSA.Create();
            m_Rsa.FromXmlString(publicKeyXml);
        }

        public Task<Version> GetCurrentVersion()
        {
            var manifestFilePath = m_ManifestFileLocation.ToPath();

            if (System.IO.File.Exists(manifestFilePath))
            {
                var manifest = new UserSettingsService().ReadSettings<SecureLibraryManifest>(
                            manifestFilePath, new BaseValueSerializer<ILocation>(null, x => Location.FromString(x)));

                return Task.FromResult(manifest.Version);
            }
            else 
            {
                return Task.FromResult(default(Version));
            }
        }

        public async Task<Version> GetLatestAvailableVersion(Version appVersion)
        {
            var lib = await FindLibrary(appVersion, null);

            return lib.Version;
        }

        public async Task Install(Version version)
        {
            try
            {
                var lib = await FindLibrary(null, version);

                var destLoc = new Location(m_ManifestFileLocation.Root, "", m_ManifestFileLocation.Segments);

                var appCenterVersionInfo = JObject.Parse(await new HttpClient().GetStringAsync(lib.DownloadUrl));

                var downloadUrl = appCenterVersionInfo.Value<string>("download_url");

                m_Logger.LogInformation($"Installing library to {destLoc.ToPath()}", true);

                await m_DestWriter.Write(destLoc, LoadLibraryFiles(downloadUrl, lib.Signature));
            }
            catch (Exception ex)
            {
                throw new UserMessageException($"Failed to install library", ex);
            }
        }

        private async Task<LibraryInfo> FindLibrary(Version appVers = null, Version libVers = null)
        {
            var libData = await new HttpClient().GetStringAsync(m_LibInfoUrl);
            var libs = new UserSettingsService().ReadSettings<LibraryCollection>(new StringReader(libData));

            if (libs == null)
            {
                throw new ArgumentNullException();
            }

            LibraryInfo lib;

            if (libVers != null)
            {
                lib = libs.Versions.FirstOrDefault(l => l.Version == libVers);

                if (lib == null)
                {
                    throw new UserMessageException($"Specified version {libVers} of the library is not available");
                }
            }
            else
            {
                lib = libs.Versions.OrderBy(l => l.Version).LastOrDefault(
                    l => appVers == null || ((l.MinimumAppVersion == null || l.MinimumAppVersion <= appVers)
                    && (l.MaximumAppVersion == null || l.MaximumAppVersion >= appVers)));

                if (lib == null)
                {
                    throw new UserMessageException($"Failed to find the version of the library which is supported by {appVers} version of the application");
                }
            }

            return lib;
        }

        private async IAsyncEnumerable<IFile> LoadLibraryFiles(string url, byte[] signature)
        {
            using (var client = new HttpClient())
            {
                var buffer = await client.GetByteArrayAsync(url);

                if (m_Rsa != null)
                {
                    if (!m_Rsa.VerifyData(buffer, signature,
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
                    {
                        throw new DigitalSignatureMismatchException(url);
                    }
                }

                using (var packageStream = new MemoryStream(buffer))
                {
                    packageStream.Seek(0, SeekOrigin.Begin);

                    using (var zip = new ZipArchive(packageStream))
                    {
                        foreach (var entry in zip.Entries)
                        {
                            if (entry.Length > 0)
                            {
                                byte[] fileBuffer;

                                using (var fileStream = entry.Open())
                                {
                                    fileBuffer = new byte[entry.Length];
                                    fileStream.Read(fileBuffer, 0, fileBuffer.Length);

                                    yield return new Data.File(Location.FromPath(entry.FullName),
                                        fileBuffer, Guid.NewGuid().ToString());
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
