//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

using System;
using System.Collections.Generic;
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
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Core.Plugin;
using Xarial.Docify.Core.Helpers;
using Xarial.Docify.Core.Data;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompiler : ICompiler
    {
        private readonly ILogger m_Logger;
        private readonly IContentTransformer m_ContentTransformer;
        private readonly BaseCompilerConfig m_Config;
        private readonly ILayoutParser m_LayoutParser;
        private readonly IIncludesHandler m_IncludesHandler;

        [ImportPlugins]
        private IEnumerable<IPreCompilePlugin> m_PreCompilePlugins = null;

        [ImportPlugins]
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

        public async Task<IFile[]> Compile(ISite site)
        {
            m_PreCompilePlugins.InvokePluginsIfAny(p => p.PreCompile(site));

            var outFiles = new List<IFile>();

            var allPages = site.GetAllPages();

            foreach (var page in allPages)
            {
                outFiles.Add(await CompilePage(page, site));

                foreach (var asset in page.Assets)
                {
                    var id = asset.Location.ToId();

                    if (PathMatcher.Matches(m_Config.CompilableAssetsFilter, id))
                    {
                        outFiles.Add(await CompileAsset(asset, site));
                    }
                    else 
                    {
                        outFiles.Add(new File(asset.Location, asset.Content));
                    }
                }
            }

            m_PostCompilePlugins.InvokePluginsIfAny(p => p.PostCompile(site));

            return outFiles.ToArray();
        }
        
        private async Task<IFile> CompilePage(IPage page, ISite site)
        {
            var model = new ContextModel(site, page);

            var content = await m_ContentTransformer.Transform(page.RawContent, page.Key, model);
            
            var layout = page.Layout;

            if (layout != null)
            {
                content = await m_LayoutParser.InsertContent(layout, content, model);
            }

            content = await m_IncludesHandler.ReplaceAll(content, site, page);

            return new File(page.Location, content);
        }

        private async Task<IFile> CompileAsset(IFile asset, ISite site)
        {
            var rawContent = asset.AsTextContent();
            var content = await m_IncludesHandler.ReplaceAll(rawContent, site, null);

            return new File(asset.Location, content);
        }
    }
}
