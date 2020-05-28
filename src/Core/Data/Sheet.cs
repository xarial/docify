//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public abstract class Sheet : ISheet
    {
        public string RawContent { get; }
        public ITemplate Layout { get; }
        public IMetadata Data { get; }
        public string Id { get; }
        public string Name { get; }

        protected Sheet(string rawContent, string name, IMetadata data, ITemplate layout, string id)
        {
            Name = name;
            RawContent = rawContent;
            Layout = layout;
            Data = data ?? new Metadata();
            Id = id;
        }
    }
}
