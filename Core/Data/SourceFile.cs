//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class SourceFile : IFile
    {
        public byte[] Content { get; }

        public ILocation Location { get; }

        public SourceFile(ILocation path, byte[] content)
        {
            Location = path;
            Content = content;
        }

        public SourceFile(ILocation path, string content)
            : this(path, FileExtension.ToByteArray(content))
        {
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }
}
