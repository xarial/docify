﻿//*********************************************************************
//Docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://docify.net
//License: https://docify.net/license/
//*********************************************************************

using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Lib.Plugins.Common.Helpers;

namespace Xarial.Docify.Lib.Plugins.Embed
{
    public class EmbedPlugin : IPlugin
    {
        private IDocifyApplication m_App;
        private ISite m_Site;

        public void Init(IDocifyApplication app)
        {
            m_App = app;

            m_App.Compiler.PreCompile += OnPreCompile;
            m_App.Includes.RegisterCustomIncludeHandler("embed", ResolveEmbedInclude);
        }

        private Task OnPreCompile(ISite site)
        {
            m_Site = site;
            return Task.CompletedTask;
        }

        private Task<string> ResolveEmbedInclude(IMetadata data, IPage page)
        {
            var embedData = data.ToObject<EmbedIncludeData>();

            var asset = AssetsHelper.FindAsset(m_Site, page, embedData.FileName);

            return Task.FromResult(asset.AsTextContent());
        }
    }
}
