//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    public interface IConfiguration : IMetadata
    {
        Environment_e Environment { get; set; }
        string WorkingFolder { get; set; }
        ILocation ComponentsFolder { get; set; }
        List<string> Components { get; set; }
        ILocation PluginsFolder { get; set; }
        List<string> Plugins { get; set; }
        ILocation ThemesFolder { get; set; }
        List<string> ThemesHierarchy { get; }
    }
}
