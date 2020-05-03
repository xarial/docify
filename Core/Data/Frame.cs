using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public abstract class Frame : IFrame
    {
        public string RawContent { get; }
        public ITemplate Layout { get; }
        public Metadata Data { get; }

        public abstract string Key { get; }

        public Frame(string rawContent, Metadata data, Template layout)
        {
            RawContent = rawContent;
            Layout = layout;
            Data = data ?? new Metadata();
        }
    }
}
