//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Base.Content
{
    public abstract class Frame : ICompilable
    {
        public string RawContent { get; }
        public Template Layout { get; }
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
