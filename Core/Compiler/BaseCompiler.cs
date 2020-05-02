//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections;
using System.Text.RegularExpressions;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Core.Compiler.Context;
using Xarial.Docify.Base.Content;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core.Plugin;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompiler : ICompiler
    {
        private readonly ILogger m_Logger;
        private readonly IContentTransformer m_ContentTransformer;
        private readonly BaseCompilerConfig m_Config;
        private readonly ILayoutParser m_LayoutParser;
        private readonly IIncludesHandler m_IncludesHandler;

        [ImportPlugin]
        private IEnumerable<IPreCompilePlugin> m_PreCompilePlugins = null;

        [ImportPlugin]
        private IEnumerable<IPostCompilePlugin> m_PostCompilePlugins = null;

        public BaseCompiler(BaseCompilerConfig config,
            ILogger logger, ILayoutParser layoutParser,
            IIncludesHandler includesHandler,
            IContentTransformer contentTransformer) 
        {
            m_Config = config;
            m_Logger = logger;
            m_LayoutParser = layoutParser;
            m_ContentTransformer = contentTransformer;
            m_IncludesHandler = includesHandler;
        }

        public async Task<IWritable[]> Compile(Site site)
        {
            m_PreCompilePlugins.InvokePluginsIfAny(p => p.PreCompile(site));

            var writables = new List<IWritable>();

            var allPages = site.GetAllPages();

            foreach (var page in allPages)
            {
                writables.Add(await CompilePage(page, site));

                foreach (var asset in page.Assets)
                {
                    if (asset is TextAsset)
                    {
                        writables.Add(await CompileAsset((TextAsset)asset, site));
                    }
                    else 
                    {
                        //TODO: change this
                        writables.Add(new Writable((asset as BinaryAsset).Content, asset.Location));
                    }
                }
            }

            m_PostCompilePlugins.InvokePluginsIfAny(p => p.PostCompile(site));

            return writables.ToArray();
        }
        
        private async Task<IWritable> CompilePage(Page page, Site site)
        {
            var model = new ContextModel(site, page);

            var content = await m_ContentTransformer.Transform(page.RawContent, page.Key, model);
            
            var layout = page.Layout;

            if (layout != null)
            {
                content = await m_LayoutParser.InsertContent(layout, content, model);
            }

            content = await m_IncludesHandler.ReplaceAll(content, site, page);

            return new Writable(content, page.Location);
        }

        private async Task<IWritable> CompileAsset(TextAsset asset, Site site)
        {
            var content = await m_IncludesHandler.ReplaceAll(asset.RawContent, site, null);

            return new Writable(content, asset.Location);
        }
    }
}
