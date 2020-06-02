using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.Docify.Core.Publisher;
using Xarial.XToolkit.Services.UserSettings;

namespace Core.Tests
{
    public class SecureLibraryCleanerTest
    {
        private RSA m_Rsa;
        
        [SetUp]
        public void Setup() 
        {
            m_Rsa = RSA.Create();
        }
        
        [Test]
        public async Task FullDirCleanTest() 
        {
            var fs = new MockFileSystem();

            fs.AddDirectory("C:\\dir\\lib");
            fs.AddFile("C:\\dir\\file1.txt", new MockFileData("f0"));
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\file1.txt", new MockFileData("f1"));
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\subdir1\\file1.txt", new MockFileData("sb1f1"));
            fs.AddFile("C:\\dir\\lib\\_themes\\theme1\\file2.txt", new MockFileData("f2"));
            fs.AddFile("C:\\dir\\lib\\_themes\\theme1\\subdir2\\file2.txt", new MockFileData("sb2f2"));
            fs.AddFile("C:\\dir\\lib\\_plugins\\plugin1\\file3.txt", new MockFileData("f3"));
            fs.AddFile("C:\\dir\\lib\\_plugins\\plugin1\\subdir3\\subdir4\\file3.txt", new MockFileData("sb3sb4f3"));

            byte[] GetSignature(string path) 
            {
                var buffer = fs.File.ReadAllBytes(path);

                return m_Rsa.SignData(buffer, 0, buffer.Length,
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            }

            var secLibMan = new SecureLibraryManifest();
            secLibMan.Components = new SecureLibraryItem[]
            {
                new SecureLibraryItem()
                {
                    Name = "comp1",
                    Files = new SecureLibraryItemFile[]
                    {
                        new SecureLibraryItemFile() 
                        {
                            Name = Location.FromPath("file1.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_components\\comp1\\file1.txt")
                        },
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("subdir1\\file1.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_components\\comp1\\subdir1\\file1.txt")
                        }
                    }
                }
            };

            secLibMan.Themes = new SecureLibraryItem[]
            {
                new SecureLibraryItem()
                {
                    Name = "theme1",
                    Files = new SecureLibraryItemFile[]
                    {
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("file2.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_themes\\theme1\\file2.txt")
                        },
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("subdir2\\file2.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_themes\\theme1\\subdir2\\file2.txt")
                        }
                    }
                }
            };

            secLibMan.Plugins = new SecureLibraryItem[]
            {
                new SecureLibraryItem()
                {
                    Name = "plugin1",
                    Files = new SecureLibraryItemFile[]
                    {
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("file3.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_plugins\\plugin1\\file3.txt")
                        },
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("subdir3\\subdir4\\file3.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_plugins\\plugin1\\subdir3\\subdir4\\file3.txt")
                        }
                    }
                }
            };

            using (var writer = fs.File.CreateText("C:\\dir\\lib\\lib.manifest")) 
            {
                new UserSettingsService().StoreSettings(secLibMan, writer, new BaseValueSerializer<ILocation>(l => l.ToId(), null));
            }
            
            var cleaner = new SecureLibraryCleaner("C:\\dir\\lib\\lib.manifest", m_Rsa.ToXmlString(false), fs);

            await cleaner.ClearDirectory(Location.FromPath("C:\\dir\\lib"));

            Assert.AreEqual(2, fs.AllDirectories.Count());
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\"));
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\dir"));
            Assert.AreEqual(1, fs.AllFiles.Count());
            Assert.AreEqual("C:\\dir\\file1.txt", fs.AllFiles.First());
        }

        [Test]
        public async Task ExtraFilesDirCleanTest()
        {
            var fs = new MockFileSystem();

            fs.AddDirectory("C:\\dir\\lib");
            fs.AddDirectory("C:\\dir\\lib\\_themes");
            fs.AddDirectory("C:\\dir\\lib\\_plugins");
            fs.AddFile("C:\\dir\\file1.txt", new MockFileData("f0"));
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\file1.txt", new MockFileData("f1"));
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\file2.txt", new MockFileData("f2"));
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\subdir1\\file1.txt", new MockFileData("sb1f1"));

            byte[] GetSignature(string path)
            {
                var buffer = fs.File.ReadAllBytes(path);

                return m_Rsa.SignData(buffer, 0, buffer.Length,
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            }
        
            var secLibMan = new SecureLibraryManifest();
            secLibMan.Components = new SecureLibraryItem[]
            {
                new SecureLibraryItem()
                {
                    Name = "comp1",
                    Files = new SecureLibraryItemFile[]
                    {
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("file1.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_components\\comp1\\file1.txt")
                        },
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("subdir1\\file1.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_components\\comp1\\subdir1\\file1.txt")
                        }
                    }
                }
            };

            using (var writer = fs.File.CreateText("C:\\dir\\lib\\lib.manifest"))
            {
                new UserSettingsService().StoreSettings(secLibMan, writer, new BaseValueSerializer<ILocation>(l => l.ToId(), null));
            }

            var cleaner = new SecureLibraryCleaner("C:\\dir\\lib\\lib.manifest", m_Rsa.ToXmlString(false), fs);

            await cleaner.ClearDirectory(Location.FromPath("C:\\dir\\lib"));

            Assert.AreEqual(5, fs.AllDirectories.Count());
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\"));
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\dir"));
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\dir\\lib"));
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\dir\\lib\\_components"));
            Assert.IsTrue(fs.AllDirectories.Contains("C:\\dir\\lib\\_components\\comp1"));
            Assert.AreEqual(2, fs.AllFiles.Count());
            Assert.IsTrue(fs.AllFiles.Contains("C:\\dir\\file1.txt"));
            Assert.IsTrue(fs.AllFiles.Contains("C:\\dir\\lib\\_components\\comp1\\file2.txt"));
        }

        [Test]
        public void ManifestDirMismatchTest() 
        {
            var fs = new MockFileSystem();

            fs.AddDirectory("C:\\dir\\lib1");
            fs.AddDirectory("C:\\dir\\lib\\_themes");
            fs.AddDirectory("C:\\dir\\lib\\_plugins");
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\file1.txt", new MockFileData("f1"));
            
            var secLibMan = new SecureLibraryManifest();
            
            using (var writer = fs.File.CreateText("C:\\dir\\lib1\\lib.manifest"))
            {
                new UserSettingsService().StoreSettings(secLibMan, writer, new BaseValueSerializer<ILocation>(l => l.ToId(), null));
            }

            var cleaner = new SecureLibraryCleaner("C:\\dir\\lib1\\lib.manifest", m_Rsa.ToXmlString(false), fs);

            Assert.ThrowsAsync<LibraryDirectoryManifestMismatchException>(() => cleaner.ClearDirectory(Location.FromPath("C:\\dir\\lib")));
        }

        [Test]
        public void ModifiedLibraryFileTest()
        {
            var fs = new MockFileSystem();

            fs.AddDirectory("C:\\dir\\lib");
            fs.AddDirectory("C:\\dir\\lib\\_themes");
            fs.AddDirectory("C:\\dir\\lib\\_plugins");
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\file1.txt", new MockFileData("f1"));
            fs.AddFile("C:\\dir\\lib\\_components\\comp1\\file2.txt", new MockFileData("f2"));

            byte[] GetSignature(string path)
            {
                var buffer = fs.File.ReadAllBytes(path);

                return m_Rsa.SignData(buffer, 0, buffer.Length,
                        HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
            }

            var secLibMan = new SecureLibraryManifest();
            secLibMan.Components = new SecureLibraryItem[]
            {
                new SecureLibraryItem()
                {
                    Name = "comp1",
                    Files = new SecureLibraryItemFile[]
                    {
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("file1.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_components\\comp1\\file1.txt")
                        },
                        new SecureLibraryItemFile()
                        {
                            Name = Location.FromPath("file2.txt"),
                            Signature = GetSignature("C:\\dir\\lib\\_components\\comp1\\file2.txt")
                        }
                    }
                }
            };

            using (var writer = fs.File.CreateText("C:\\dir\\lib\\lib.manifest"))
            {
                new UserSettingsService().StoreSettings(secLibMan, writer, new BaseValueSerializer<ILocation>(l => l.ToId(), null));
            }

            var cleaner = new SecureLibraryCleaner("C:\\dir\\lib\\lib.manifest", m_Rsa.ToXmlString(false), fs);

            fs.File.WriteAllText("C:\\dir\\lib\\_components\\comp1\\file2.txt", "f2-mod");

            Assert.ThrowsAsync<LibraryFileModifiedException>(() => cleaner.ClearDirectory(Location.FromPath("C:\\dir\\lib")));
        }
    }
}
