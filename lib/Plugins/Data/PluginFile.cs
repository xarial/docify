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

namespace Xarial.Docify.Lib.Plugins.Data
{
    public class PluginFile : IFile
    {
        public byte[] Content { get; }
        public ILocation Location { get; }
        public string Id { get; }

        public PluginFile(string content, ILocation loc) 
            : this(ContentExtension.ToByteArray(content), loc)
        {
        }

        public PluginFile(byte[] content, ILocation loc)
            : this(content, loc, Guid.NewGuid().ToString())
        {
        }

        public PluginFile(byte[] content, ILocation loc, string id) 
        {
            Content = content;
            Location = loc;
            Id = id;
        }
    }
}
