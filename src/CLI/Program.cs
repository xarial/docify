//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.CLI.Options;
using Xarial.Docify.CLI.Properties;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Core.Plugin.Extensions;
using Xarial.Docify.Core.Publisher;
using Xarial.Docify.Core.Tools;
using Xarial.XToolkit.Services.UserSettings;

namespace Xarial.Docify.CLI
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var parser = new Parser(p =>
            {
                p.CaseInsensitiveEnumValues = true;
                p.AutoHelp = true;
                p.EnableDashDash = true;
                p.HelpWriter = Console.Out;
                p.IgnoreUnknownArguments = false;
            });

            bool isError = false;

            BuildOptions buildOpts = null;
            ServeOptions serveOpts = null;
            GenerateLibraryManifestOptions genManOpts = null;
            LibraryOptions libOpts = null;

            parser.ParseArguments<BuildOptions, ServeOptions, GenerateLibraryManifestOptions, LibraryOptions>(args)
                .WithParsed<BuildOptions>(o => buildOpts = o)
                .WithParsed<ServeOptions>(o => serveOpts = o)
                .WithParsed<GenerateLibraryManifestOptions>(o => genManOpts = o)
                .WithParsed<LibraryOptions>(o => libOpts = o)
                .WithNotParsed(e => isError = true);

            if (!isError)
            {
                DocifyEngine engine = null;

                if (buildOpts != null)
                {
                    engine = new DocifyEngine(buildOpts.SourceDirectories.ToArray(),
                        buildOpts.OutputDirectory, buildOpts.Library?.ToArray(), buildOpts.SiteUrl, buildOpts.Environment);
                }
                else if (serveOpts != null)
                {
                    throw new NotSupportedException("This option is not supported");
                }
                else if (genManOpts != null)
                {
                    await GenerateLibraryManifest(genManOpts);
                }
                else if (libOpts != null) 
                {
                    await InstallLibrary(libOpts);
                }

                if (buildOpts != null)
                {
                    await engine.Build();
                }
            }
        }

        private static async Task GenerateLibraryManifest(GenerateLibraryManifestOptions genManOpts)
        {
            var manGen = new ManifestGenerator(new LocalFileSystemFileLoader());
            
            string publicKeyXml;

            var maninfest = await manGen.CreateManifest(Location.FromPath(genManOpts.LibraryPath),
                new Version(genManOpts.Version),
                genManOpts.CertificatePath, genManOpts.CertificatePassword, out publicKeyXml);

            new UserSettingsService().StoreSettings(maninfest,
                Path.Combine(genManOpts.LibraryPath, "library.manifest"),
                new BaseValueSerializer<ILocation>(x => x.ToId(), null));

            if (!string.IsNullOrEmpty(genManOpts.PublicKeyFile))
            {
                await File.WriteAllTextAsync(genManOpts.PublicKeyFile, publicKeyXml);
            }
        }

        private static async Task InstallLibrary(LibraryOptions opts) 
        {
            if (opts.CheckForUpdates)
            {
                //TODO: check for updates
            }
            else if (opts.Install)
            {
                var destLoc = Location.Library.DefaultLibraryManifestFilePath;
                destLoc = destLoc.Copy("", destLoc.Path);

                var libInstaller = new LibraryInstaller();
                
                var libData = await new HttpClient().GetStringAsync("http://localhost:8081/version.json");
                var libColl = new UserSettingsService().ReadSettings<LibraryCollection>(new StringReader(libData));

                var lib = libInstaller.FindLibrary(libColl, typeof(DocifyEngine).Assembly.GetName().Version, opts.Version);
                
                await libInstaller.InstallLibrary(Location.FromUrl(lib.DownloadUrl), destLoc, 
                    new WebZipFileLoader(lib.Signature, Resources.standard_library_public_key),
                    new LocalFileSystemPublisher(new PublisherExtension()));
            }
        }
    }
}
