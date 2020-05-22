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
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;

namespace Xarial.Docify.Core.Plugin.Extensions
{
    public interface ICompilerExtension
    {
        Task<string> WritePageContent(string content, IMetadata data, string url);
        Task PreCompile(ISite site);
        void RenderCodeBlock(string rawCode, string lang, string args, StringBuilder html);
        void RenderImage(StringBuilder html);
        void RenderUrl(StringBuilder html);
        Task<IFile> PostCompileFile(IFile file);
        Task PostCompile();
    }

    public class CompilerExtension : ICompilerExtension
    {   
        public event PreCompileDelegate RequestPreCompile;
        public event RenderCodeBlockDelegate RequestRenderCodeBlock;
        public event RenderImageDelegate RequestRenderImage;
        public event RenderUrlDelegate RequestRenderUrl;
        public event WritePageContentDelegate RequestWritePageContent;
        public event PostCompileFileDelegate RequestPostCompileFile;
        public event PostCompileDelegate RequestPostCompile;

        public Task PostCompile()
        {
            return RequestPostCompile.Invoke();
        }

        public Task<IFile> PostCompileFile(IFile file)
        {
            return RequestPostCompileFile.Invoke(file);
        }

        public Task PreCompile(ISite site)
        {
            return RequestPreCompile.Invoke(site);
        }

        public void RenderCodeBlock(string rawCode, string lang, string args, StringBuilder html)
        {
            RequestRenderCodeBlock.Invoke(rawCode, lang, args, html);
        }

        public void RenderImage(StringBuilder html)
        {
            RequestRenderImage.Invoke(html);
        }

        public void RenderUrl(StringBuilder html)
        {
            RequestRenderUrl.Invoke(html);
        }

        public Task<string> WritePageContent(string content, IMetadata data, string url)
        {
            return RequestWritePageContent.Invoke(content, data, url);
        }
    }
}
