//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.XToolkit.Services.UserSettings;
using Xarial.XToolkit.Services.UserSettings.Attributes;

namespace Xarial.Docify.Core.Data
{
    public class SecureLibraryItem
    {
        public string Name { get; set; }
        public SecureLibraryItemFile[] Files { get; set; }
    }

    public class SecureLibraryItemFile 
    {
        public ILocation Name { get; set; }
        public byte[] Signature { get; set; }
    }

    public class SecureLibraryManifestVersionTransformer : BaseUserSettingsVersionsTransformer
    {
        public SecureLibraryManifestVersionTransformer() 
        {
        }
    }

    [UserSettingVersion("1.0.0", typeof(SecureLibraryManifestVersionTransformer))]
    public class SecureLibraryManifest
    {
        public Version Version { get; set; }
        public SecureLibraryItem[] Components { get; set; }
        public SecureLibraryItem[] Themes { get; set; }
        public SecureLibraryItem[] Plugins { get; set; }
    }
}
