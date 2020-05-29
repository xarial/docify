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
