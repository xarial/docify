//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System.Collections.Generic;

namespace Xarial.Docify.Lib.Plugins.SeoValidator
{
    public class SeoValidatorPluginSettings 
    {
        public string[] Scope { get; set; }
        public bool TreatErrorAsWarning { get; set; } = true;
        public Dictionary<string, Dictionary<string, object>> Validators { get; set; }
    }
}
