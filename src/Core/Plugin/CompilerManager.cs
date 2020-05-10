using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xarial.Docify.Base;
using Xarial.Docify.Base.Data;
using Xarial.Docify.Base.Plugins;
using Xarial.Docify.Base.Services;
using Xarial.Docify.Core.Plugin.Extensions;

namespace Xarial.Docify.Core.Plugin
{
    public class CompilerManager : ICompilerManager
    {
        public event AddFilesPostCompileDelegate AddFilesPostCompile;
        public event PreCompileDelegate PreCompile;
        public event RenderCodeBlockDelegate RenderCodeBlock;
        public event RenderImageDelegate RenderImage;
        public event RenderUrlDelegate RenderUrl;
        public event WritePageContentDelegate WritePageContent;

        public ICompiler Instance { get; }
        public IContentTransformer ContentTransformer { get; }

        private readonly CompilerExtension m_Ext;

        public CompilerManager(ICompiler compiler, IContentTransformer contTransf, CompilerExtension ext) 
        {
            Instance = compiler;
            ContentTransformer = contTransf;
            m_Ext = ext;
            m_Ext.RequestAddFilesPostCompile += OnRequestAddFilesPostCompile;
            m_Ext.RequestPreCompile += OnRequestPreCompile;
            m_Ext.RequestRenderCodeBlock += OnRequestRenderCodeBlock;
            m_Ext.RequestRenderImage += OnRequestRenderImage;
            m_Ext.RequestRenderUrl += OnRequestRenderUrl;
            m_Ext.RequestWritePageContent += OnRequestWritePageContent;
        }

        private async Task OnRequestPreCompile(ISite site)
        {
            if (PreCompile != null) 
            {
                foreach (PreCompileDelegate del in PreCompile.GetInvocationList()) 
                {
                    await del.Invoke(site);
                }
            }
        }

        private async IAsyncEnumerable<IFile> OnRequestAddFilesPostCompile()
        {
            if (AddFilesPostCompile != null) 
            {
                foreach (AddFilesPostCompileDelegate del in AddFilesPostCompile.GetInvocationList()) 
                {
                    await foreach (var file in del.Invoke()) 
                    {
                        yield return file;
                    }
                }
            }
        }

        private async Task<string> OnRequestWritePageContent(string content, IMetadata data, string url)
        {
            var res = content;

            if (WritePageContent != null) 
            {
                foreach (WritePageContentDelegate del in WritePageContent.GetInvocationList()) 
                {
                    res = await del.Invoke(res, data, url);
                }
            }

            return res;
        }

        private void OnRequestRenderUrl(StringBuilder html)
        {
            RenderUrl?.Invoke(html);
        }

        private void OnRequestRenderImage(StringBuilder html)
        {
            RenderImage?.Invoke(html);
        }

        private void OnRequestRenderCodeBlock(string rawCode, string lang, string args, StringBuilder html)
        {
            RenderCodeBlock?.Invoke(rawCode, lang, args, html);
        }
    }
}
