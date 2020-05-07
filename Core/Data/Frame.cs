using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(Key) + "}")]
    public abstract class Frame : IFrame
    {
        public string RawContent { get; }
        public ITemplate Layout { get; }
        public IMetadata Data { get; }

        public abstract string Key { get; }

        public Frame(string rawContent, IMetadata data, Template layout)
        {
            RawContent = rawContent;
            Layout = layout;
            Data = data ?? new Metadata();
        }
    }
}
