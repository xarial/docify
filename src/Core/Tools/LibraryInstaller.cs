using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Data;
using Xarial.Docify.Core.Exceptions;
using Xarial.XToolkit.Services.UserSettings;
using Xarial.XToolkit.Services.UserSettings.Attributes;

namespace Xarial.Docify.Core.Tools
{
    public class LibraryCollectionVersionTransformer : BaseUserSettingsVersionsTransformer
    {
    }

    [UserSettingVersion("1.0.0", typeof(LibraryCollectionVersionTransformer))]
    public class LibraryCollection
    {
        public Library[] Versions { get; set; }
    }

    public class Library 
    {
        public Version MinimumAppVersion { get; set; }
        public Version MaximumAppVersion { get; set; }
        public Version Version { get; set; }
        public string DownloadUrl { get; set; }
        public string Signature { get; set; }
    }

    public class LibraryInstaller
    {
        public Library FindLibrary(LibraryCollection libs, Version appVers, Version libVers = null) 
        {
            if (libs == null) 
            {
                throw new ArgumentNullException();
            }

            Library lib;

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
                    l => (l.MinimumAppVersion == null || l.MinimumAppVersion <= appVers) 
                    && (l.MaximumAppVersion == null || l.MaximumAppVersion >= appVers));

                if (lib == null) 
                {
                    throw new UserMessageException($"Failed to find the version of the library which is supported by {appVers} version of the application");
                }
            }

            return lib;
        }

        public async Task InstallLibrary(ILocation srcLoc, ILocation destLoc, 
            IFileLoader srcLoader, IPublisher destWriter)
        {
            try
            {
                await destWriter.Write(destLoc, srcLoader.LoadFolder(srcLoc, null));
            }
            catch (Exception ex)
            {
                throw new UserMessageException($"Failed to install library", ex);
            }
        }
    }
}
