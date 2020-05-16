﻿//*********************************************************************
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
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Compiler
{
    public class BaseCompiler : ICompiler
    {
        private readonly ILogger m_Logger;
        private readonly IContentTransformer m_ContentTransformer;
        private readonly BaseCompilerConfig m_Config;
        private readonly ILayoutParser m_LayoutParser;
        private readonly IIncludesHandler m_IncludesHandler;

        private readonly ICompilerExtension m_Ext;


        public BaseCompiler(BaseCompilerConfig config,
            ILogger logger, ILayoutParser layoutParser,
            IIncludesHandler includesHandler,
            IContentTransformer contentTransformer, ICompilerExtension ext) 
        {
            m_Config = config;
            m_Logger = logger;
            m_LayoutParser = layoutParser;
            m_ContentTransformer = contentTransformer;
            m_IncludesHandler = includesHandler;

            m_Ext = ext;
        }

        public async IAsyncEnumerable<IFile> Compile(ISite site)
        {
            await m_Ext.PreCompile(site);

            await foreach (var file in CompileAll(site.MainPage, site, Location.Empty))
            {
                yield return await m_Ext.PostCompileFile(file);
            }

            await m_Ext.PostCompile();
        }

        private async IAsyncEnumerable<IFile> CompileAll(IPage page, ISite site, ILocation baseLoc) 
        {
            const string PAGE_FILE_NAME = "index.html";

            ILocation thisLoc;

            if (!baseLoc.IsEmpty())
            {
                thisLoc = baseLoc.Combine(new Location(PAGE_FILE_NAME, page.Name));
            }
            else
            {
                thisLoc = new Location(PAGE_FILE_NAME);
            }

            ILocation pageLoc;

            if (!System.IO.Path.HasExtension(page.Name))
            {
                pageLoc = thisLoc;
            }
            else 
            {
                pageLoc = baseLoc.Combine(new Location(page.Name));
            }

            yield return await CompilePage(page, site, pageLoc);

            await foreach (var asset in CompileAssets(page, page, site, thisLoc))
            {
                yield return asset;
            }

            foreach (var child in page.SubPages) 
            {
                await foreach (var subPage in CompileAll(child, site, thisLoc)) 
                {
                    yield return subPage;
                }
            }
        }

        private async IAsyncEnumerable<IFile> CompileAssets(IAssetsFolder folder, IPage page, ISite site, ILocation baseLoc)
        {
            foreach (var asset in folder.Assets)
            {
                var thisLoc = baseLoc.Combine(new Location(asset.FileName));
                
                if (PathMatcher.Matches(m_Config.CompilableAssetsFilter, thisLoc.ToId()))
                {
                    yield return await CompileAsset(asset, site, page, thisLoc);
                }
                else
                {
                    yield return new File(thisLoc, asset.Content, asset.Id);
                }
                
            }

            foreach (var subFolder in folder.Folders) 
            {
                var folderLoc = baseLoc.Combine(new Location("", subFolder.Name));
                await foreach (var subFolderAsset in CompileAssets(subFolder, page, site, folderLoc)) 
                {
                    yield return subFolderAsset;
                }
            }
        }
        
        private async Task<IFile> CompilePage(IPage page, ISite site, ILocation loc)
        {
            var url = loc.ToUrl();
            var model = new ContextModel(site, page, url);

            var content = await m_ContentTransformer.Transform(page.RawContent, page.Id, model);
            
            var layout = page.Layout;

            if (layout != null)
            {
                content = await m_LayoutParser.InsertContent(layout, content, model);
            }

            content = await m_IncludesHandler.ReplaceAll(content, site, page, url);

            content = await m_Ext.WritePageContent(content, page.Data, url);

            return new File(loc, ContentExtension.ToByteArray(content), page.Id);
        }

        private async Task<IFile> CompileAsset(IAsset asset, ISite site, IPage page, ILocation loc)
        {
            var url = loc.ToUrl();

            var rawContent = asset.AsTextContent();
            var content = await m_IncludesHandler.ReplaceAll(rawContent, site, page, url);

            return new File(loc, ContentExtension.ToByteArray(content), asset.Id);
        }
    }
}
