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

namespace Xarial.Docify.Base.Content
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
    public class Asset : IFile
    {
        public static Asset FromTextContent(string content, Location loc)
        {
            var buffer = DataConverter.ToByteArray(content);

            return new Asset(loc, buffer);
        }

        public byte[] Content { get; }
        public Location Location { get; }

        public Asset(Location loc, byte[] content)
        {
            Location = loc;
            Content = content;
        }
    }
}
