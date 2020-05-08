using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public abstract class Frame : IFrame
    {
        public string RawContent { get; }
        public ITemplate Layout { get; }
        public IMetadata Data { get; }
        public string Key { get; }
        public string Name { get; }

        public Frame(string rawContent, string name, IMetadata data, Template layout)
        {
            Name = name;
            RawContent = rawContent;
            Layout = layout;
            Data = data ?? new Metadata();
            Key = Guid.NewGuid().ToString();
        }
    }
}
