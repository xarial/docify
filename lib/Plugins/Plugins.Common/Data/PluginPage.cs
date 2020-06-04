//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Collections.Generic;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;

namespace Xarial.Docify.Lib.Plugins.Common.Data
{
    public class PluginPage : IPage
    {
        public List<IPage> SubPages { get; }
        public string RawContent { get; }
        public ITemplate Layout { get; }
        public IMetadata Data { get; }
        public string Id { get; }
        public string Name { get; }
        public List<IAsset> Assets { get; }
        public List<IAssetsFolder> Folders { get; }

        public PluginPage(string name, string rawContent, string id, IMetadata data, ITemplate template = null)
        {
            SubPages = new List<IPage>();
            Assets = new List<IAsset>();
            Folders = new List<IAssetsFolder>();

            Name = name;
            RawContent = rawContent;
            Id = id;
            Data = data;
            Layout = template;
        }
    }
}
