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
        string Environment { get; set; }
        List<string> Components { get; set; }
        List<string> Plugins { get; set; }
        List<string> ThemesHierarchy { get; }
    }
}
