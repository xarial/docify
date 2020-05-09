using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Template : Sheet, ITemplate
    {
        public Template(string name, string rawContent,
            IMetadata data = null, Template baseTemplate = null)
            : base(rawContent, name, data, baseTemplate)
        {
        }
    }
}
