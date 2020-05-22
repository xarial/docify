//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using Xarial.Docify.Base.Context;

namespace Xarial.Docify.Core.Compiler.Context
{
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
