using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Compiler.Context
{
    public interface IContextAsset 
    {
        byte[] Content { get; }
        string Name { get; }
    }

    public class ContextAsset : IContextAsset
    {
        public  byte[] Content { get; }
        public string Name { get; }

        public ContextAsset(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }
    }
}
