//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using System.Text;
using Xarial.XToolkit.Services.UserSettings;
using Xarial.XToolkit.Services.UserSettings.Attributes;

namespace Xarial.Docify.Core.Library
{
    public class LibraryCollectionVersionTransformer : BaseUserSettingsVersionsTransformer
    {
    }

    [UserSettingVersion("1.0.0", typeof(LibraryCollectionVersionTransformer))]
    public class LibraryCollection
    {
        public LibraryInfo[] Versions { get; set; }
    }
}
