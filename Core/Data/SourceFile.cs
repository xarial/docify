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
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class SourceFile : IFile
    {
        public byte[] Content { get; }

        public Location Location { get; }

        public SourceFile(Location path, byte[] content)
        {
            Location = path;
            Content = content;
        }

        public override string ToString()
        {
            return Location.ToString();
        }
    }
}
