using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Xarial.Docify.Base.Services
{
    public interface ILibraryInstaller
    {
        Task<Version> GetCurrentVersion();
        Task<Version> GetLatestAvailableVersion(Version appVersion);
        Task Install(Version version);
    }
}
