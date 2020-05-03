using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Template : Frame, ITemplate
    {
        public string Name { get; }
        public override string Key => Name;

        public Template(string name, string rawContent,
            Metadata data = null, Template baseTemplate = null)
            : base(rawContent, data, baseTemplate)
        {
            Name = name;
        }
    }
}
