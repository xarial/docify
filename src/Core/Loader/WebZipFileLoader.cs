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
using System.IO.Packaging;
using System.Net.Http;
using System.Security.Cryptography;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Exceptions;

namespace Xarial.Docify.Core.Loader
{
    public class WebZipFileLoader : IFileLoader
    {
        private readonly string m_Signature;
        private readonly RSA m_Rsa;

        public WebZipFileLoader() 
        {
        }

        public WebZipFileLoader(string signature, string publicKeyXml) 
        {
            m_Signature = signature;

            m_Rsa = RSA.Create();
            m_Rsa.FromXmlString(publicKeyXml);
        }

        public IAsyncEnumerable<ILocation> EnumSubFolders(ILocation location)
        {
            throw new NotImplementedException();
        }

        public bool Exists(ILocation location)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<IFile> LoadFolder(ILocation location, string[] filters)
        {
            using (var client = new HttpClient()) 
            {
                var buffer = await client.GetByteArrayAsync(location.ToUrl());

                if (m_Rsa != null)
                {
                    if (!m_Rsa.VerifyData(buffer, Convert.FromBase64String(m_Signature),
                            HashAlgorithmName.SHA256, RSASignaturePadding.Pss))
                    {
                        throw new DigitalSignatureMismatchException(location);
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
