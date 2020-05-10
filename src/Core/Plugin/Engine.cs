using System;
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
    public delegate Task<string> ResolveCustomIncludeDelegate(IMetadata data, IPage page);

    public interface IEngine
    {
        IIncludesHandlerManager Includes { get; }
        ICompilerManager Compiler { get; }
        IComposerManager Composer { get; }
        IPublisherManager Publisher { get; }
    }

    public interface IIncludesHandlerManager 
    {
        IIncludesHandler Instance { get; }
        void RegisterCustomIncludeHandler(string includeName, ResolveCustomIncludeDelegate handler);
    }

    public delegate IAsyncEnumerable<IFile> AddFilesPostCompileDelegate();
    public delegate Task PreCompileDelegate(ISite site);
    public delegate void RenderCodeBlockDelegate(string rawCode, string lang, string args, StringBuilder html);
    public delegate void RenderImageDelegate(StringBuilder html);
    public delegate void RenderUrlDelegate(StringBuilder html);
    public delegate Task<string> WritePageContentDelegate(string content, IMetadata data, string url);

    public interface ICompilerManager 
    {
        event AddFilesPostCompileDelegate AddFilesPostCompile;
        event PreCompileDelegate PreCompile;
        event RenderCodeBlockDelegate RenderCodeBlock;
        event RenderImageDelegate RenderImage;
        event RenderUrlDelegate RenderUrl;
        event WritePageContentDelegate WritePageContent;

        ICompiler Instance { get; }
        IContentTransformer ContentTransformer { get; }
    }

    public interface IComposerManager 
    {
        IComposer Instance { get; }
    }

    public delegate Task PostPublishDelegate(ILocation loc);
    public delegate Task<Extensions.PrePublishResult> PrePublishFileDelegate(ILocation outLoc, IFile file);

    public interface IPublisherManager 
    {
        event PostPublishDelegate PostPublish;
        event PrePublishFileDelegate PrePublishFile;

        IPublisher Instance { get; }
    }

    public class IncludesHandlerManager : IIncludesHandlerManager
    {
        public IIncludesHandler Instance { get; }
                
        private readonly Dictionary<string, ResolveCustomIncludeDelegate> m_CustomIncludesHandlers;
        private readonly IncludesHandlerExtension m_Ext;

        public IncludesHandlerManager(IIncludesHandler instance, IncludesHandlerExtension ext) 
        {
            Instance = instance;
            m_Ext = ext;

            m_Ext.RequestResolveInclude += OnRequestResolveInclude;

            m_CustomIncludesHandlers = new Dictionary<string, ResolveCustomIncludeDelegate>(
                StringComparer.CurrentCultureIgnoreCase);

        }

        private async Task<string> OnRequestResolveInclude(string includeName, IMetadata metadata, IPage page)
        {
            if (m_CustomIncludesHandlers.TryGetValue(includeName, out ResolveCustomIncludeDelegate handler))
            {
                return await handler.Invoke(metadata, page);
            }
            else 
            {
                throw new Exception($"Include '{includeName}' is not registered");
            }
        }

        public void RegisterCustomIncludeHandler(string includeName, ResolveCustomIncludeDelegate handler)
        {
            if (!m_CustomIncludesHandlers.ContainsKey(includeName))
            {
                m_CustomIncludesHandlers.Add(includeName, handler);
            }
            else
            {
                throw new Exception($"Include '{includeName}' already registered with other plugin");
            }
        }
    }

    public class PublisherManager : IPublisherManager
    {
        public IPublisher Instance { get; }

        public event PostPublishDelegate PostPublish;
        public event PrePublishFileDelegate PrePublishFile;

        private readonly PublisherExtension m_Ext;

        public PublisherManager(IPublisher inst, PublisherExtension ext) 
        {
            Instance = inst;
            m_Ext = ext;

            m_Ext.RequestPostPublish += OnRequestPostPublish;
            m_Ext.RequestPrePublishFile += OnRequestPrePublishFile;
        }

        private async Task<Extensions.PrePublishResult> OnRequestPrePublishFile(ILocation outLoc, IFile file)
        {
            Extensions.PrePublishResult curRes = new Extensions.PrePublishResult()
            {
                File = file,
                SkipFile = false
            };

            if (PrePublishFile != null) 
            {
                foreach (PrePublishFileDelegate del in PrePublishFile.GetInvocationList()) 
                {
                    var thisRes = await del.Invoke(outLoc, curRes.File);
                    curRes.File = thisRes.File;
                    curRes.SkipFile &= thisRes.SkipFile;
                }
            }

            return curRes;
        }

        private async Task OnRequestPostPublish(ILocation loc)
        {
            if (PostPublish != null) 
            {
                foreach (PostPublishDelegate del in PostPublish.GetInvocationList()) 
                {
                    await del.Invoke(loc);
                }
            }
        }
    }

    public class ComposerManager : IComposerManager
    {
        public IComposer Instance { get; }
        private readonly ComposerExtension m_Ext;

        public ComposerManager(IComposer inst, ComposerExtension ext)
        {
            Instance = inst;
            m_Ext = ext;
        }
    }

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

    public class Engine : IEngine
    {
        public IIncludesHandlerManager Includes { get; }
        public ICompilerManager Compiler { get; }
        public IComposerManager Composer { get; }
        public IPublisherManager Publisher { get; }

        public Engine(IIncludesHandlerManager includes, 
            ICompilerManager compiler, 
            IComposerManager composer, 
            IPublisherManager publisher)
        {
            Includes = includes;
            Compiler = compiler;
            Composer = composer;
            Publisher = publisher;
        }
    }

    public class IncludesHandlerExtension : IIncludesHandlerExtension
    {
        public event Func<string, IMetadata, IPage, Task<string>> RequestResolveInclude;

        public Task<string> ResolveInclude(string includeName, IMetadata md, IPage page)
        {
            return RequestResolveInclude.Invoke(includeName, md, page);
        }
    }

    public class ComposerExtension : IComposerExtension 
    {
    }

    public class PublisherExtension : IPublisherExtension
    {
        public event PostPublishDelegate RequestPostPublish;
        public event PrePublishFileDelegate RequestPrePublishFile;

        public Task PostPublish(ILocation loc)
        {
            return RequestPostPublish.Invoke(loc);
        }

        public Task<Extensions.PrePublishResult> PrePublishFile(ILocation outLoc, IFile file)
        {
            return RequestPrePublishFile.Invoke(outLoc, file);
        }
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
