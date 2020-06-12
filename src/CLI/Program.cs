//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using CommandLine;
using CommandLine.Text;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.CLI.Options;
using Xarial.Docify.CLI.Properties;
using Xarial.Docify.Core;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Loader;
using Xarial.Docify.Core.Logger;
using Xarial.Docify.Core.Plugin.Extensions;
using Xarial.Docify.Core.Publisher;
using Xarial.Docify.Core.Tools;
using Xarial.XToolkit.Services.UserSettings;

namespace Xarial.Docify.CLI
{
    internal class Program
    {
        private const string STANDARD_LIB_URL = "https://docify.net/library.json";

        private static ILogger m_Logger;

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

            BuildOptions buildOpts = null;
            ServeOptions serveOpts = null;
            GenerateLibraryManifestOptions genManOpts = null;
            LibraryOptions libOpts = null;

            parser.ParseArguments<BuildOptions, ServeOptions, GenerateLibraryManifestOptions, LibraryOptions>(args)
                .WithParsed<BuildOptions>(o => buildOpts = o)
                .WithParsed<ServeOptions>(o => serveOpts = o)
                .WithParsed<GenerateLibraryManifestOptions>(o => genManOpts = o)
                .WithParsed<LibraryOptions>(o => libOpts = o)
                .WithNotParsed(e => Environment.Exit(1));

            try
            {
                if (buildOpts != null)
                {
                    var engine = new DocifyEngine(buildOpts.SourceDirectories.ToArray(),
                        buildOpts.OutputDirectory, buildOpts.Library?.ToArray(), 
                        buildOpts.SiteUrl, buildOpts.Environment, buildOpts.Verbose);

                    m_Logger = engine.Resolve<ILogger>();

                    await engine.Build();
                }
                else if (serveOpts != null)
                {
                    throw new NotSupportedException("This option is not supported");
                }
                else if (genManOpts != null)
                {
                    m_Logger = new ConsoleLogger(genManOpts.Verbose);
                    //TODO: create service
                    await GenerateLibraryManifest(genManOpts);
                }
                else if (libOpts != null)
                {
                    m_Logger = new ConsoleLogger(libOpts.Verbose);
                    //TODO: create service
                    await InstallLibrary(libOpts);
                }
            }
            catch (Exception ex)
            {
                string fullLog;
                var err = ParseError(ex, out fullLog);
                System.Diagnostics.Trace.WriteLine(fullLog, "Xarial.Docify");

                m_Logger.LogError(err);
                m_Logger.LogError(fullLog, true);

                Environment.Exit(1);
            }
        }

        private static string ParseError(Exception ex, out string fullLog) 
        {
            var res = new StringBuilder();
            var fullLogBuilder = new StringBuilder();

            void ProcessException(Exception ex) 
            {
                fullLogBuilder.AppendLine(ex.Message);
                fullLogBuilder.AppendLine(ex.StackTrace);

                if (ex is IUserMessageException) 
                {
                    res.AppendLine(ex.Message);
                }

                if (ex.InnerException != null) 
                {
                    ProcessException(ex.InnerException);
                }
            }

            ProcessException(ex);

            if (res.Length == 0) 
            {
                res.Append("Generic error");
            }

            fullLog = fullLogBuilder.ToString();

            return res.ToString();
        }

        private static async Task GenerateLibraryManifest(GenerateLibraryManifestOptions genManOpts)
        {
            var manGen = new ManifestGenerator(new LocalFileSystemFileLoader(m_Logger));
            
            string publicKeyXml;

            var maninfest = await manGen.CreateManifest(Location.FromPath(genManOpts.LibraryPath),
                genManOpts.Version,
                genManOpts.CertificatePath, genManOpts.CertificatePassword, out publicKeyXml);

            new UserSettingsService().StoreSettings(maninfest,
                Path.Combine(genManOpts.LibraryPath, "library.manifest"),
                new BaseValueSerializer<ILocation>(x => x.ToId(), null));

            if (!string.IsNullOrEmpty(genManOpts.PublicKeyFile))
            {
                await System.IO.File.WriteAllTextAsync(genManOpts.PublicKeyFile, publicKeyXml);
            }
        }

        private static async Task InstallLibrary(LibraryOptions opts) 
        {
            var libInstaller = new LibraryInstaller();

            var libData = await new HttpClient().GetStringAsync(STANDARD_LIB_URL);
            var libColl = new UserSettingsService().ReadSettings<LibraryCollection>(new StringReader(libData));

            var lib = libInstaller.FindLibrary(libColl, typeof(DocifyEngine).Assembly.GetName().Version, opts.Version);

            Version curLibVers = null;

            var manifestFilePath = Location.Library.DefaultLibraryManifestFilePath.ToPath();

            if (System.IO.File.Exists(manifestFilePath))
            {
                var manifest = new UserSettingsService().ReadSettings<SecureLibraryManifest>(
                            manifestFilePath, new BaseValueSerializer<ILocation>(null, x => Location.FromString(x)));

                curLibVers = manifest.Version;
            }

            if (opts.CheckForUpdates)
            {
                if (curLibVers != null && curLibVers >= lib.Version)
                {
                    m_Logger.LogInformation($"Currently installed library '{curLibVers}' is up-to-date");
                }
                else if(curLibVers != null)
                {
                    m_Logger.LogInformation($"New version '{lib.Version}' is available");
                }
                else
                {
                    m_Logger.LogInformation($"Version '{lib.Version}' is available");
                }
            }
            else if (opts.Install)
            {
                var destLoc = Location.Library.DefaultLibraryManifestFilePath;
                destLoc = destLoc.Copy("", destLoc.Path);

                await libInstaller.InstallLibrary(Location.FromUrl(lib.DownloadUrl), destLoc, 
                    new WebZipFileLoader(lib.Signature, Resources.standard_library_public_key),
                    new LocalFileSystemPublisher(new PublisherExtension(), m_Logger, 
                    new SecureLibraryCleaner(Location.Library.DefaultLibraryManifestFilePath.ToPath(), 
                        Resources.standard_library_public_key)));

                if (curLibVers != null && curLibVers == lib.Version)
                {
                    m_Logger.LogInformation($"Reinstalled '{lib.Version}' version of the library");
                }
                else if (curLibVers != null)
                {
                    if(curLibVers > lib.Version) 
                    {
                        m_Logger.LogInformation($"Library was downgraded to version '{lib.Version}'");
                    }
                    else
                    {
                        m_Logger.LogInformation($"Library was updated to version '{lib.Version}'");
                    }
                }
                else 
                {
                    m_Logger.LogInformation($"Installed '{lib.Version}' version of the library");
                }
            }
        }
    }
}
