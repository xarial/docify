//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Base.Content
{
    public class BinaryAsset : Asset
    {
        public byte[] Content { get; }

        public BinaryAsset(byte[] content, Location loc) : base(loc)
        {
            Content = content;
        }
    }
}
