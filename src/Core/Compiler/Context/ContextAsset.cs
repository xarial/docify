//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
    public class ContextAsset : IContextAsset
    {
        public byte[] Content { get; }
        public string Name { get; }

        public ContextAsset(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }
    }
}
