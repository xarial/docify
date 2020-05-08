using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Core.Data
{
    public class Asset : IAsset
    {
        public string Name { get; }
        public byte[] Content { get; }

        public Asset(string name, byte[] content) 
        {
            Name = name;
            Content = content;
        }
    }
}
