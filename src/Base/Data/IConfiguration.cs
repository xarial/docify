//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Base.Data
{
    public interface IConfiguration : IMetadata
    {
        string Environment { get; set; }
        List<string> Components { get; set; }
        List<string> Plugins { get; set; }
        List<string> ThemesHierarchy { get; }
    }
}
