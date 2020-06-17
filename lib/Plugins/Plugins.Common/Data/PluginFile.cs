//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using System.Diagnostics;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Data
{
    [DebuggerDisplay("{" + nameof(Location) + "}")]
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

        public PluginFile(string content, ILocation loc, string id)
            : this(ContentExtension.ToByteArray(content), loc, id)
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
