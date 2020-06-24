//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

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
