﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Data
{
    public class PluginAsset : IAsset
    {
        public byte[] Content { get; }
        public string FileName { get; }

        public string Id { get; }

        public PluginAsset(string content, string name)
            : this(ContentExtension.ToByteArray(content), name)
        {
        }

        public PluginAsset(byte[] content, string name)
            : this(content, name, Guid.NewGuid().ToString())
        {
        }

        public PluginAsset(byte[] content, string name, string id)
        {
            Content = content;
            FileName = name;
            Id = id;
        }
    }
}
