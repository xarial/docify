using System;
using System.Collections.Generic;
using System.Text;

namespace Xarial.Docify.Core.Compiler.Context
{
    public interface IContextAsset 
    {
        string Name { get; }
    }

    public class ContextTextAsset : IContextAsset
    {
        public string Content { get; }
        public string Name { get; }

        public ContextTextAsset(string name, string content) 
        {
            Name = name;
            Content = content;
        }
    }

    public class ContextBinaryAsset : IContextAsset
    {
        public  byte[] Content { get; }
        public string Name { get; }

        public ContextBinaryAsset(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }
    }
}
