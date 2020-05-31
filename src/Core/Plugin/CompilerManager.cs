//*********************************************************************
//docify
//Copyright(C) 2020 Xarial Pty Limited
//Product URL: https://www.docify.net
//License: https://github.com/xarial/docify/blob/master/LICENSE
//*********************************************************************

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
        public event PreCompileDelegate PreCompile;
        public event RenderCodeBlockDelegate RenderCodeBlock;
        public event RenderImageDelegate RenderImage;
        public event RenderUrlDelegate RenderUrl;
        public event WritePageContentDelegate WritePageContent;
        public event PostCompileFileDelegate PostCompileFile;
        public event PostCompileDelegate PostCompile;

        public ICompiler Instance { get; }

        public IDynamicContentTransformer DynamicContentTransformer { get; }
        public IStaticContentTransformer StaticContentTransformer { get; }

        private readonly CompilerExtension m_Ext;

        public CompilerManager(ICompiler compiler,
            IStaticContentTransformer staticContTransf,
            IDynamicContentTransformer dynContTransf,
            CompilerExtension ext)
        {
            Instance = compiler;
            StaticContentTransformer = staticContTransf;
            DynamicContentTransformer = dynContTransf;

            m_Ext = ext;
            m_Ext.RequestPreCompile += OnRequestPreCompile;
            m_Ext.RequestRenderCodeBlock += OnRequestRenderCodeBlock;
            m_Ext.RequestRenderImage += OnRequestRenderImage;
            m_Ext.RequestRenderUrl += OnRequestRenderUrl;
            m_Ext.RequestWritePageContent += OnRequestWritePageContent;
            m_Ext.RequestPostCompile += OnRequestPostCompile;
            m_Ext.RequestPostCompileFile += OnRequestPostCompileFile;
        }

        private async Task<IFile> OnRequestPostCompileFile(IFile file)
        {
            if (PostCompileFile != null)
            {
                foreach (PostCompileFileDelegate del in PostCompileFile.GetInvocationList())
                {
                    file = await del.Invoke(file);
                }
            }

            return file;
        }

        private async Task OnRequestPostCompile()
        {
            if (PostCompile != null)
            {
                foreach (PostCompileDelegate del in PostCompile.GetInvocationList())
                {
                    await del.Invoke();
                }
            }
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
