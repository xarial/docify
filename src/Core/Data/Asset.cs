//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Diagnostics;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    [DebuggerDisplay("{" + nameof(FileName) + "}")]
    public class Asset : IAsset
    {
        public string FileName { get; }
        public byte[] Content { get; }

        public string Id { get; }

        public Asset(string fileName, byte[] content, string id)
        {
            FileName = fileName;
            Content = content;
            Id = id;
        }
    }
}
