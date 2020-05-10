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
        IAsyncEnumerable<IFile> AddFilesPostCompile();
        Task PreCompile(ISite site);
        void RenderCodeBlock(string rawCode, string lang, string args, StringBuilder html);
        void RenderImage(StringBuilder html);
        void RenderUrl(StringBuilder html);
    }

    public class CompilerExtension : ICompilerExtension
    {
        public event AddFilesPostCompileDelegate RequestAddFilesPostCompile;
        public event PreCompileDelegate RequestPreCompile;
        public event RenderCodeBlockDelegate RequestRenderCodeBlock;
        public event RenderImageDelegate RequestRenderImage;
        public event RenderUrlDelegate RequestRenderUrl;
        public event WritePageContentDelegate RequestWritePageContent;

        public IAsyncEnumerable<IFile> AddFilesPostCompile()
        {
            return RequestAddFilesPostCompile.Invoke();
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
